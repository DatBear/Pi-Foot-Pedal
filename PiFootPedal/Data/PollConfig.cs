namespace PiFootPedal.Data;

public class PollConfig
{
    public bool IsVerbose { get; set; }
    public HashSet<int> ButtonPins { get; set; } = new();
    public Dictionary<int, ButtonAction> ButtonActions { get; set; } = new();
}