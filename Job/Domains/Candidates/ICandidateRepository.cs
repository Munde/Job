using Job.Shared;
using Job.Shared.Models.Candidates;

namespace Job.Domains.Candidates
{
    public interface ICandidateRepository
    {
        /// <summary>
        /// Gets candidates records from the datastore based on the 
        /// specified page and the size specified
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        ValueTask<IEnumerable<Candidate>> GetAllAsync(int page,int size);

        /// <summary>
        /// Gets specific candidate from the datastore 
        /// based on the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<Candidate> GetAsync(Guid id);

        /// <summary>
        /// Creates candidate record intp the datastore and if the 
        /// candidate exists then it allows data to be updated using email
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        ValueTask<Response> CreateOrUpdateAsync(Candidate candidate);

        /// <summary>
        /// Deletes candidate from the datastore using the specified candidate id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<Response> DeleteAsync(Guid id);
    }
}
