using Job.Domains.Candidates;
using Job.Repositories;
using Job.Shared.Models.Candidates;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Job.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateRepository repository;

        public CandidatesController(ICandidateRepository repository)
        {
            repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<ActionResult<IEnumerable<Candidate>>> GetAsync(
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            if(page <= 1)
                throw new ArgumentException("Invalid page number provided", nameof(page));

            if(size < 1)
                throw new ArgumentException("Invalid page size provided", nameof(page));

            var candidates = await repository.GetAllAsync(page, size);

            if(!candidates.Any())
                return NotFound();

            return Ok(candidates);
        }

        [HttpGet("{id}")]
        [ProducesErrorResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetAsync(Guid id)
        {
            var candidate = await repository.GetAsync(id);

            if(candidate is null) 
                return NotFound();

            return Ok(candidate);
        }




    }
}
