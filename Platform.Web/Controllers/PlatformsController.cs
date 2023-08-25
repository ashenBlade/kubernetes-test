using Mapster;
using Microsoft.AspNetCore.Mvc;
using Platform.Web.Dto;
using Platform.Web.Services;

namespace Platform.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _repository;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(IPlatformRepository repository, ILogger<PlatformsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IEnumerable<ReadPlatformDto>> ReadAllPlatformsAsync(CancellationToken token)
    {
        var items = await _repository.GetAllPlatformsAsync(token);
        return items.Adapt<IEnumerable<ReadPlatformDto>>();
    }

    [HttpPost("")]
    public async Task<ReadPlatformDto> CreatePlatformAsync(CreatePlatformDto dto, CancellationToken token)
    {
        var platform = await _repository.CreatePlatformAsync(dto.Name, dto.Publisher, dto.Cost, token);
        _logger.LogInformation("Создана новая платформа {PlatformName}. Присвоен ID {PlatformId}", platform.Name, platform.Id);
        return platform.Adapt<ReadPlatformDto>();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReadPlatformDto>> FindPlatformByIdAsync(int id, CancellationToken token)
    {
        var found = await _repository.FindPlatformByIdAsync(id, token);
        if (found is not null)
        {
            return Ok(found);
        }

        return NotFound();
    }
}