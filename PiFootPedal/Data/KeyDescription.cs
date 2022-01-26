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
        if (PreSleepMs.HasValue)
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

    public void Write()
    {
        char[] str = {
            Modifier.HasValue ? (char)Modifier : '\0', '\0', (char)Key, '\0', '\0', '\0', '\0', '\0'
        };
        var bytes = str.SelectMany(BitConverter.GetBytes).ToArray();
        File.WriteAllBytes("/dev/hidg0", bytes);
    }

    public void WriteReleaseAll()
    {
        char[] str = {
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0'
        };
        var bytes = str.SelectMany(BitConverter.GetBytes).ToArray();
        File.WriteAllBytes("/dev/hidg0", bytes);
    }
}