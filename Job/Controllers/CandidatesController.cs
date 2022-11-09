using Job.Domains.Candidates;
using Job.Repositories;
using Job.Shared.Dtos.Candidates;
using Job.Shared;
using Job.Shared.Models.Candidates;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Job.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateRepository _repository;

        public CandidatesController(ICandidateRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<ActionResult<IEnumerable<Candidate>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            if(page < 1)
                throw new ArgumentException("Invalid page number provided", nameof(page));

            if(size < 1)
                throw new ArgumentException("Invalid page size provided", nameof(page));

            var candidates = await _repository.GetAllAsync(page, size);

            if(!candidates.Any())
                return NotFound();

            return Ok(candidates);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> Get(Guid id)
        {
            var candidate = await _repository.GetAsync(id);

            if(candidate is null) 
                return NotFound();

            return Ok(candidate);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async ValueTask<ActionResult<Response>> Post([FromBody] CandidateDto dto)
        {
            try
            {
                if (dto == null)
                    return NoContent();

                var candidate = new Candidate()
                {
                    Firstname = dto.Firstname,
                    Lastname = dto.Lastname,
                    Email = dto.Email,
                    CallTimeInterval = dto.CallTimeInterval,
                    Comment = dto.Comment,
                    GithubLink = dto.GithubLink,
                    LinkedInProfile = dto.LinkedInProfile,
                    Phone = dto.Phone
                };

                await _repository.CreateOrUpdateAsync(candidate);
                return CreatedAtAction(nameof(Get), new { candidate.Id }, dto );
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<ActionResult<Response>> Delete(Guid id)
        {
            var candidate = await _repository.GetAsync(id);
            if(candidate is null)
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
