using Commands.Web.Models;
using Commands.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commands.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepository _repo;
    private readonly ILogger<CommandsController> _logger;

    public CommandsController(ICommandRepository repo, ILogger<CommandsController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet("")]
    public Task<IEnumerable<Command>> GetAllCommandsAsync(CancellationToken token)
    {
        return _repo.GetAllCommandsAsync(token);
    }
}