using PiFootPedal.Enums;

namespace PiFootPedal.Data;

public class KeyDescription
{
    public ModifierKeys? Modifier { get; set; }
    public Keys Key { get; set; }
    public int? PreSleepMs { get; set; }
    public int? PostSleepMs { get; set; }

    public async Task PreSleep()
    {
        if(PreSleepMs.HasValue)
            await Task.Delay(PreSleepMs.Value);
    }

    public async Task PostSleep()
    {
        if (PostSleepMs.HasValue)
            await Task.Delay(PostSleepMs.Value);
    }

    public override string ToString()
    {
        return $"{(Modifier.HasValue ? $"{Modifier} + " : string.Empty)}{Key}";
    }
}