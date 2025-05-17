namespace SurveyBasket.Contracts.Requests;

public sealed record CreatePollRequest(
    string Name,
    string Description);