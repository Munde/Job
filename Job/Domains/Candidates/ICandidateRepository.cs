using Job.Shared;
using Job.Shared.Models.Candidates;

namespace Job.Domains.Candidates
{
    public interface ICandidateRepository
    {
        ValueTask<IEnumerable<Candidate>> GetAllAsync(int page,int size);
        ValueTask<Candidate> GetAsync(Guid id);
        ValueTask<Response> CreateOrUpdateAsync(Candidate candidate);
        ValueTask<Response> UpdateAsync(Guid id);
    }
}
