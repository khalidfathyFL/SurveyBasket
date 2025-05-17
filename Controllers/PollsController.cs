using Mapster;
using SurveyBasket.Contracts.Requests;
using SurveyBasket.Contracts.Responses;
using SurveyBasket.Mapping;
using SurveyBasket.ServicesContracts;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;


    [HttpGet("")]
    public IActionResult GetAll()
    {
        return Ok(_pollService.GetAll().Adapt<PollResponse>());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var poll = _pollService.GetById(id);
        
        return poll is null ? NotFound(): Ok(poll.Adapt<PollResponse>());
     
    }

    [HttpPost("")]
    public IActionResult Create([FromBody] CreatePollRequest poll)
    {
        if (poll is null)
            return BadRequest("Poll cannot be null");
        
        
        var newPoll =  _pollService.Add(poll.Adapt<Poll>());

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Poll poll)
    {
       var isUpdated = _pollService.Update(id, poll);
        if (!isUpdated)
            return NotFound();
        
        return NoContent();

    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var isDeleted= _pollService.Delete(id);
        if (!isDeleted)
            return NotFound();
        
        return NoContent();
    }

}
