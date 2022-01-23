using PiFootPedal.Services;

namespace PiFootPedal.Controllers;

public class ConfigController : BaseController
{
    private readonly ConfigService _configService;
    private readonly PollService _pollService;

    public ConfigController(ConfigService configService, PollService pollService)
    {
        _configService = configService;
        _pollService = pollService;
    }
    
}