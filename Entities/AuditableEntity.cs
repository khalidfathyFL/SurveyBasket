namespace SurveyBasket.Entities;

public class AuditableEntity
{
    /// <summary>
    ///     Identifier of the user who created the poll.
    ///     Represents a foreign key relation to the ApplicationUser entity.
    /// </summary>
    public string? CreatedById { get; set; }

    /// <summary>
    ///     The user who created the poll.
    ///     Represents the relationship between the poll and the application user who authored it.
    /// </summary>
    public ApplicationUser CreatedBy { get; set; }


    /// <summary>
    ///     Identifier of the user who last updated the poll.
    ///     References to the associated ApplicationUser entity.
    /// </summary>
    public string? UpdatedById { get; set; }


    /// <summary>
    ///     Indicates the date and time when the poll was last updated.
    ///     It is nullable and reflects modifications made to the poll information.
    /// </summary>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    ///     The date and time when the poll was created.
    ///     Automatically set to the current UTC date and time when the poll is instantiated.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     References to the user who last updated the poll.
    ///     Populated with an instance of ApplicationUser when the poll is modified,
    ///     or null if the poll has not been updated.
    /// </summary>
    public ApplicationUser UpdatedBy { get; set; }
}