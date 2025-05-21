using Mapster;

namespace SurveyBasket.Controllers;

/// <summary>
///     API Controller for managing polls/surveys in the system.
///     Provides endpoints for creating, reading, updating, and deleting polls.
///     Route: api/[controller] will map to "api/polls"
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;

    /// <summary>
    ///     Constructor for PollsController
    ///     Uses dependency injection to get the poll service
    /// </summary>
    /// <param name="pollService">Service for handling poll-related operations</param>
    public PollsController(IPollService pollService)
    {
        _pollService = pollService;
    }

    /// <summary>
    ///     Gets all polls in the system
    ///     GET: api/polls
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>List of all polls</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls);
    }

    /// <summary>
    ///     Gets a specific poll by its ID
    ///     GET: api/polls/5
    /// </summary>
    /// <param name="id">The ID of the poll to retrieve</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The requested poll or NotFound if it doesn't exist</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetByIdAsync(id, cancellationToken);
        if (poll is null)
            return NotFound();
        return Ok(poll);
    }

    /// <summary>
    ///     Creates a new poll
    ///     POST: api/polls
    /// </summary>
    /// <param name="request">The poll data to create</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The created poll and its location</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        // Map request to Poll entity
        var poll = request.Adapt<Poll>();

        // Create the poll
        var createdPoll = await _pollService.CreateAsync(poll, cancellationToken);

        // Return created poll with location header
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPoll.Id },
            createdPoll);
    }

    /// <summary>
    ///     Updates an existing poll
    ///     PUT: api/polls/5
    /// </summary>
    /// <param name="id">The ID of the poll to update</param>
    /// <param name="request">The updated poll data</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>No content if successful, NotFound if poll doesn't exist</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        // Map request to Poll entity and set the ID
        var poll = request.Adapt<Poll>();
        poll.Id = id;

        var success = await _pollService.UpdateAsync(poll, cancellationToken);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    ///     Deletes a specific poll
    ///     DELETE: api/polls/5
    /// </summary>
    /// <param name="id">The ID of the poll to delete</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>No content if successful, NotFound if poll doesn't exist</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _pollService.DeleteAsync(id, cancellationToken);
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    ///     Toggles the publish status of a poll
    ///     PUT: api/polls/5/toggle-publish
    /// </summary>
    [HttpPut("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var success = await _pollService.TogglePublishAsync(id, cancellationToken);
        if (!success)
            return NotFound();
        return NoContent();
    }
}