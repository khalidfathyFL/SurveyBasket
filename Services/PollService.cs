using SurveyBasket.ServicesContracts;

namespace SurveyBasket.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> Polls =
    [
        new Poll
        {
            Id = 1,
            Name = "Customer Satisfaction Survey",
            Description = "A survey to gather feedback on customer satisfaction regarding our services."
        },

        new Poll
        {
            Id = 2,
            Name = "Employee Engagement Poll",
            Description = "A poll to assess employee engagement and workplace satisfaction."
        },

        new Poll
        {
            Id = 3,
            Name = "New Product Feedback",
            Description = "Collecting opinions on the recently launched product features and usability."
        },

        new Poll
        {
            Id = 4,
            Name = "Event Participation",
            Description = "Poll to know how many team members plan to attend the upcoming annual event."
        }
    ];

    public IEnumerable<Poll> GetAll() => Polls;
   

    public Poll? GetById(int id) => Polls.SingleOrDefault(p => p.Id == id);
    public Poll Add(Poll poll)
    {
        poll.Id = Polls.Max(p => p.Id) + 1;
        Polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var existingPoll = GetById(id);
        if (existingPoll is null)
        {
            return false;
        }
        existingPoll.Name = poll.Name;
        existingPoll.Description = poll.Description;
        return true;
    }

    public bool Delete(int id)
    {
        var poll = GetById(id);
        if (poll is null)
        {
            return false;
        }
        Polls.Remove(poll);
        return true;
    }
}
    
