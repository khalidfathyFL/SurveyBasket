namespace SurveyBasket.Mapping;

// here if we will use manual mapping but i will use mapster  for mapping

//public static class ContractMapping
//{
//    public static PollResponse MapToPollResponse(this Poll poll)
//    {
//        return new PollResponse
//        {
//            Id = poll.Id,
//            Name = poll.Name,
//            Description = poll.Description
//        };
//    }
//    public static IEnumerable<PollResponse> MapToPollResponse(this IEnumerable<Poll> polls)
//    {
//        return polls.Select(p => p.MapToPollResponse());
//    }
//    public static Poll MapToPoll(this CreatePollRequest request)
//    {
//        return new Poll
//        {
//            Name = request.Name,
//            Description = request.Description
//        };
//    }
//}