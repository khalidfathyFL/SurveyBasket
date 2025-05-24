namespace SurveyBasket.Errors;

public class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email/password");

    public static readonly Error RefreshTokenNotFound =
        new("User.RefreshTokenNotFound", "Refresh token not found or expired");

    public static readonly Error InvalidToken = new("User.InvalidToken", "Token is invalid or expired");
    public static Error UserNotFound => new("User.NotFound", "User was not found in the system.");

    public static Error RefreshTokenInvalid =>
        new("Auth.RefreshToken.Invalid", "The provided refresh token is invalid or inactive.");
}