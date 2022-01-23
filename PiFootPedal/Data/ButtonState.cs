using System.Device.Gpio;

namespace PiFootPedal.Data;

public class ButtonState
{
    public ButtonState(int pin, PinEventTypes pinEvent, long lastChanged)
    {
        Pin = pin;
        PinEvent = pinEvent;
        LastChanged = lastChanged;
    }

    public int Pin { get; set; }
    public PinEventTypes PinEvent { get; set; }
    public long LastChanged { get; set; }
    public int PressCounter { get; set; }
    
}