using System.Device.Gpio;
using System.Diagnostics;
using PiFootPedal.Data;
using PiFootPedal.Enums;

namespace PiFootPedal.Services;

public class PollService
{
    private readonly ConfigService _configService;
    private readonly Dictionary<int, ButtonState> _pinChanges = new();

    public bool IsDebug => _configService.GetConfig()?.IsVerbose ?? true;
    public PollModes PollMode { get; set; }

    private GpioController _gpio;
    public readonly SetupInfo SetupInfo = new();

    private bool _ledOn;
    private const int LedPin = 17;

    private static readonly int[] ValidButtonGpioPins =
    {
        4, /*17,*/27, 22, 5, 6, 13, 19, 26, //todo add 17 back in, it's an led currently
        18, 23, 24, 25, 12, 16, 20
    };

    //not sure why this constant is wrong in TimeSpan... but it 100% is.
    private const long TicksPerMs = TimeSpan.TicksPerMillisecond * 100;
    private const int DebounceMs = 60;

    public PollService(ConfigService configService)
    {
        _configService = configService;
        PollMode = PollModes.SendKeys;
        
    }

    public async Task Start()
    {
        int buttonPin = 5;
        _gpio = new GpioController();
        _gpio.OpenPin(LedPin, PinMode.Output);
        RegisterButton(buttonPin);

        var i = 0;
        while (true)
        {
            await Task.Delay(500);
            Console.WriteLine($"test {i++}");
        }
    }

    private bool RegisterButton(int pin)
    {
        if (_gpio.IsPinOpen(pin)) return false;

        var pullUpSupport = _gpio.IsPinModeSupported(pin, PinMode.InputPullUp);
        _gpio.OpenPin(pin, pullUpSupport ? PinMode.InputPullUp : PinMode.Input);
        _pinChanges[pin] = new ButtonState(pin, PinEventTypes.Rising, Stopwatch.GetTimestamp());
        _gpio.RegisterCallbackForPinValueChangedEvent(pin, PinEventTypes.Rising | PinEventTypes.Falling, OnPinChanged);
        return true;
    }

    private void UnregisterButton(int pin)
    {
        var open = _gpio.IsPinOpen(pin);
        Console.WriteLine($"closing pin {pin}, open? {open}");
        _pinChanges.Remove(pin);
        if (open)
        {
            _gpio.UnregisterCallbackForPinValueChangedEvent(pin, OnPinChanged);
            _gpio.ClosePin(pin);
        }
            
    }


    private void OnPinChanged(object sender, PinValueChangedEventArgs pin)
    {
        var ts = Stopwatch.GetTimestamp();
        var button = _pinChanges[pin.PinNumber];

        if (ts - button.LastChanged > TicksPerMs * DebounceMs)
        {
            button.PinEvent = pin.ChangeType;
            Console.WriteLine($"{pin.PinNumber} is {button.PinEvent} {(ts - button.LastChanged) / (TicksPerMs)}");
            button.LastChanged = ts;

            _ledOn = pin.ChangeType == PinEventTypes.Falling;
            _gpio.Write(LedPin, ((_ledOn) ? PinValue.High : PinValue.Low));

            if (PollMode == PollModes.Setup)
            {
                Console.WriteLine($"Adding pin {pin.PinNumber}");
                SetupInfo.ButtonPins.Add(pin.PinNumber);
                return;
            }


            var config = _configService.GetConfig();
            if (config == null) return;

            if (config.ButtonActions.TryGetValue(pin.PinNumber, out var action))
            {
                if (button.PinEvent == PinEventTypes.Falling)
                {
                    //button pressed
                    Task.Run(async () => await action.Press(button));
                    button.PressCounter++;
                    //Console.WriteLine(string.Join(", ", action.Keys.Select(x => $"{(x.Modifier.HasValue ? $"{x.Modifier} + " : string.Empty)}{x.Key}")));
                }
                else
                {
                    //button released
                    Task.Run(async () => await action.Release(button));
                }
            }
            else
            {
                if (IsDebug)
                {
                    Console.WriteLine($"Error: Could not find action for pin {pin.PinNumber}");
                }
            }
        }
    }

    public async Task<bool> EnterSetupMode()
    {
        if (PollMode == PollModes.Setup) return false;
        SetupInfo.Clear();

        foreach (var pin in ValidButtonGpioPins)
        {
            Console.WriteLine($"registering pin {pin}");
            UnregisterButton(pin);
            RegisterButton(pin);
            
        }

        PollMode = PollModes.Setup;
        return true;
    }

    public async Task<bool> ExitSetupMode(bool saveChanges = true, bool includeMapping = true)
    {
        if (PollMode != PollModes.Setup) return false;


        var config = _configService.GetConfig();
        if (saveChanges)
        {
            foreach (var pin in ValidButtonGpioPins.Except(SetupInfo.ButtonPins))
            {
                UnregisterButton(pin);
            }
            
            if (includeMapping && config != null)
            {
                var newConfig = new PollConfig();
                newConfig.ButtonPins = SetupInfo.ButtonPins.ToHashSet();
                var newPins = newConfig.ButtonPins.ToArray();
                var i = 0;
                var configActions = config.ButtonActions;
                foreach (var pin in config.ButtonPins)
                {
                    if (newPins.Length <= i) break;
                    newConfig.ButtonActions[newPins[i]] = configActions[pin];
                    i++;
                }

                _configService.Save(newConfig);
            }
        }
        else if(config != null)
        {
            foreach (var pin in ValidButtonGpioPins.Except(config.ButtonPins))
            {
                UnregisterButton(pin);
            }
        }

        PollMode = PollModes.SendKeys;
        return true;
    }
}