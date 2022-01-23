using Microsoft.AspNetCore.Mvc;
using PiFootPedal.Enums;
using PiFootPedal.Services;

namespace PiFootPedal.Controllers;

public class SetupController : BaseApiController
{
    private readonly ConfigService _configService;
    private readonly PollService _pollService;

    public SetupController(ConfigService configService, PollService pollService)
    {
        _configService = configService;
        _pollService = pollService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        var success = await _pollService.EnterSetupMode();
        return Ok(success);
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        var success = await _pollService.ExitSetupMode();
        return Ok(success);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (_pollService.PollMode != PollModes.Setup)
        {
            return BadRequest($"Setup mode hasn't been started");
        }

        return Ok(_pollService.SetupInfo);
    }
}