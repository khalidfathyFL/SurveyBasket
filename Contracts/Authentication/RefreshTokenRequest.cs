namespace SurveyBasket.Contracts.Authentication;

public record RefreshTokenRequest(string Token, string RefreshToken);

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
    }
}