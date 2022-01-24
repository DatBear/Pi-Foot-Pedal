using Microsoft.AspNetCore.Mvc;
using PiFootPedal.Data;
using PiFootPedal.Services;

namespace PiFootPedal.Controllers;

public class ConfigController : BaseApiController
{
    private readonly ConfigService _configService;
    private readonly PollService _pollService;

    public ConfigController(ConfigService configService, PollService pollService)
    {
        _configService = configService;
        _pollService = pollService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return Ok(_configService.GetConfig());
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] PollConfig config)
    {
        if (config == null) return BadRequest();
        var saved = _configService.Save(config);
        return saved != null ? Ok(saved) : BadRequest();
    }
    
}