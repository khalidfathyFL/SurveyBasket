using SurveyBasket.Persistence;

namespace SurveyBasket.Services;

/// <summary>
///     Implementation of the IPollService interface.
///     Handles all poll-related operations using Entity Framework Core for data access.
/// </summary>
public class PollService : IPollService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///     Constructor for PollService
    ///     Uses dependency injection to get the database context
    /// </summary>
    /// <param name="context">The database context for accessing poll data</param>
    public PollService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Retrieves all polls from the database asynchronously
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>A collection of all polls</returns>
    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Polls.ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Retrieves a specific poll by its ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the poll to retrieve</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The requested poll or null if not found</returns>
    public async Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Polls.FindAsync([id], cancellationToken);
    }

    /// <summary>
    ///     Creates a new poll in the database asynchronously
    /// </summary>
    /// <param name="poll">The poll data to create</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The created poll with its assigned ID</returns>
    public async Task<Poll> CreateAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        _context.Polls.Add(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    /// <summary>
    ///     Updates an existing poll in the database asynchronously
    /// </summary>
    /// <param name="poll">The updated poll data</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>True if the update was successful, false if the poll wasn't found</returns>
    public async Task<bool> UpdateAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Entry(poll).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PollExistsAsync(poll.Id, cancellationToken))
                return false;
            throw;
        }
    }

    /// <summary>
    ///     Deletes a poll from the database asynchronously
    /// </summary>
    /// <param name="id">The ID of the poll to delete</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>True if the deletion was successful, false if the poll wasn't found</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll == null)
            return false;

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    ///     Toggles the publish status of a poll asynchronously
    /// </summary>
    /// <param name="id">The ID of the poll to toggle</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>True if the toggle was successful, false if the poll wasn't found</returns>
    public async Task<bool> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync([id], cancellationToken);
        if (poll == null)
            return false;

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
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