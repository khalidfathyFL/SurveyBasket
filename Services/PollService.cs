using Mapster;
using SurveyBasket.Errors;
using SurveyBasket.Persistence;

namespace SurveyBasket.Services;

/// <summary>
/// Provides implementation for managing polls through CRUD operations and other poll-related
/// functionalities, such as toggling publish state, using Entity Framework Core.
/// </summary>
public class PollService : IPollService
{
    /// <summary>
    ///     Represents the database context used for accessing and managing poll-related data within the application.
    ///     Provides access to the underlying Entity Framework Core database operations and entities.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Service for managing poll-related operations
    /// Provides methods for creating, retrieving, updating, deleting,
    /// and toggling the publish state of polls.
    /// </summary>
    public PollService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all polls from the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation if required.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all polls.</returns>
    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<PollResponse>>(polls.Adapt<IEnumerable<PollResponse>>());
    }

    /// <summary>
    /// Retrieves a poll by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the poll to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the operation, if required.</param>
    /// <returns>A result containing the requested poll data or an error if not found.</returns>
    public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll is null)
            return Result.Failure<PollResponse>(PollErrors.NotFound);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    /// <summary>
    /// Creates a new poll in the database asynchronously
    /// </summary>
    /// <param name="poll">The poll data to create</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>A result containing the created poll response or an error</returns>
    public async Task<Result<PollResponse>> CreateAsync(PollRequest poll, CancellationToken cancellationToken = default)
    {
        _context.Polls.Add(poll.Adapt<Poll>());
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(poll.Adapt<PollResponse>());
    }

    /// <summary>
    ///     Updates an existing poll in the database asynchronously
    /// </summary>
    /// <param name="id">The unique identifier of the poll to update</param>
    /// <param name="pollRequest">The updated poll data containing new values for the poll</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>A result indicating success or failure of the update operation</returns>
    public async Task<Result<bool>> UpdateAsync(int id, PollRequest pollRequest,
        CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll is null)
            return Result.Failure<bool>(PollErrors.NotFound);

        // Update properties directly on the existing entity
        poll.Title = pollRequest.Title;
        poll.Summary = pollRequest.Summary;
        poll.StartsAt = pollRequest.StartsAt;
        poll.EndsAt = pollRequest.EndsAt;

        // Add any other properties that need to be updated

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    /// <summary>
    ///     Deletes a poll from the database asynchronously.
    /// </summary>
    /// <param name="id">The ID of the poll to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A Result object containing true if the deletion was successful, or false if the poll was not found.</returns>
    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.NotFound);

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    /// <summary>
    ///     Toggles the publish status of a poll asynchronously.
    /// </summary>
    /// <param name="id">The ID of the poll to toggle.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>True if the toggle was successful, false if the poll was not found.</returns>
    public async Task<Result<bool>> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll == null)
            return Result.Failure<bool>(PollErrors.NotFound);

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }

    /// <summary>
    ///     Private helper method to check if a poll exists in the database
    /// </summary>
    /// <param name="id">The ID of the poll to check</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>True if the poll exists, false otherwise</returns>
    private async Task<bool> PollExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Polls.AnyAsync(p => p.Id == id, cancellationToken);
    }
}