namespace PiFootPedal.Services;

public class LogService
{
    private static ConfigService? _configService;

    public static void WriteLine(string line)
    {
        if (_configService?.GetConfig()?.IsVerbose ?? true)
        {
            Console.WriteLine(line);
        }
    }

    public static void Setup(ConfigService configService)
    {
        _configService = configService;
    }
}