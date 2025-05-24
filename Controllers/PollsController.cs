using Mapster;

namespace SurveyBasket.Controllers;

/// <summary>
///     API Controller for managing polls/surveys in the system.
///     Provides endpoints for creating, reading, updating, and deleting polls.
///     Route: api/[controller] will map to "api/polls"
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PollsController : ApiControllerBase
{
    private readonly IPollService _pollService;

    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error, StatusCodes.Status404NotFound);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value.Adapt<PollResponse>())
            : HandleError(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleError(result.Error, StatusCodes.Status404NotFound);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleError(result.Error, StatusCodes.Status404NotFound);
    }

    [HttpPut("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleError(result.Error, StatusCodes.Status404NotFound);
    }
}