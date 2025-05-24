namespace SurveyBasket.Controllers;

/// <summary>
///     Base controller providing common functionality for all API controllers
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    ///     Handles error responses with appropriate status codes
    /// </summary>
    /// <param name="error">The error details</param>
    /// <param name="statusCode">Optional status code, defaults to 400 Bad Request</param>
    protected IActionResult HandleError(Error error, int statusCode = StatusCodes.Status400BadRequest)
    {
        return Problem(
            title: error.Code,
            detail: error.Description,
            statusCode: statusCode);
    }
}