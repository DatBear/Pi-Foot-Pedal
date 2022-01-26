using PiFootPedal.Enums;

namespace PiFootPedal.Data;

public class ButtonAction
{
    public ButtonActionType Type { get; set; }
    public List<KeyDescription> Keys { get; set; } = new();
    
    public async Task Press(ButtonState state)
    {
        switch (Type)
        {
            case ButtonActionType.Press:
                foreach (var key in Keys)
                {
                    await Press(key, true);
                }
                break;
            case ButtonActionType.Hold:
                foreach (var key in Keys)
                {
                    await Press(key);
                }
                break;
            case ButtonActionType.RepeatPress:
                if (state.PressCounter % 2 == 0) return;
                foreach (var key in Keys)
                {
                    await Press(key);
                }
                break;
            
        }
    }

    public async Task Release(ButtonState state)
    {
        switch (Type)
        {
            case ButtonActionType.Press:
                return;
            case ButtonActionType.Hold:
                foreach (var key in Keys)
                {
                    await Release(key);
                }
                break;
            case ButtonActionType.RepeatPress:
                if (state.PressCounter % 2 == 1) return;
                foreach (var key in Keys)
                {
                    await Release(key);
                }
                break;
        }
    }

    private async Task Press(KeyDescription key, bool release = false)
    {
        await key.PreSleep();
        key.Write();
        if (release)
        {
            key.WriteReleaseAll();
        }
        await key.PostSleep();
    }

    private async Task Release(KeyDescription key)
    {
        await key.PreSleep();
        key.WriteReleaseAll();
        await key.PostSleep();
    }
}