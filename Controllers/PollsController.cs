
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
        return Ok(_pollService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var poll = _pollService.GetById(id);
        
        return poll is null ? NotFound(): Ok(poll);
     
    }

    [HttpPost("")]
    public IActionResult Create([FromBody] Poll poll)
    {
        if (poll is null)
            return BadRequest("Poll cannot be null");
        
        
        _pollService.Add(poll);

        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);
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
