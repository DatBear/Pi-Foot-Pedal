namespace PiFootPedal.Data;

public class SetupInfo
{
    public readonly HashSet<int> ButtonPins = new();

    public void Clear()
    {
        ButtonPins.Clear();
    }
}