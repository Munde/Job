using CsvHelper;

using Job.Domains.Candidates;
using Job.Mappers;
using Job.Shared;
using Job.Shared.Models.Candidates;

using LazyCache;

using System.Text;

namespace Job.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly string _datastore = "candidates.csv";
        private readonly string _path;
        private readonly IWebHostEnvironment _environment;
        private readonly IAppCache _cache;
        private readonly string _cacheKey = "candidates_cache";

        public CandidateRepository(IWebHostEnvironment environment, IAppCache cache)
        {
            _environment = environment;
            _cache = cache;
            _path = $"{_environment.WebRootPath}/{_datastore}";
        }


        /// <summary>
        /// adds or updates candidate record in the data store
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public async ValueTask<Response> CreateOrUpdateAsync(Candidate candidate)
        {
            try
            {
                //Get all the records and cast to list 
                List<Candidate> candidates = (List<Candidate>)GetCsvRecords();

                //check if the candidate details exists using given email
                var existingCandidate = candidates.FirstOrDefault(x => x.Email == candidate.Email);

                //if candidate found then lets update 
                if (existingCandidate is not null)
                {
                    //remove the existing instance of candidate from memory 
                    candidates.Remove(existingCandidate);

                    //then add the new one with updated details  but we need to preserve the candidate Id
                    candidate.Id = existingCandidate.Id;
                    candidates.Add(candidate);
                }
                else
                {
                    //add the new candidate the list of existsing candidates in memory
                    candidates.Add(candidate);
                }

                //create a stream write by providing path and the encoding type 
                using StreamWriter sw = new(_path, false, new UTF8Encoding(true));
                using CsvWriter cw = new(sw);

                //Write header of the csv using the candidate fields then move to next record
                cw.WriteHeader<Candidate>();
                cw.NextRecord();


                foreach (var cand in candidates)
                {
                    //write the candidate details to the csv
                    cw.WriteRecord(cand);
                    cw.NextRecord();
                }
                //invalidate the cache so as it can fetch new data 
                _cache.Remove(_cacheKey);

                var response = new Response(true, "Candidate details recorded successfully");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }


        /// <summary>
        /// Deletes specified candidate from the datastore by specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Response> DeleteAsync(Guid id)
        {
            //Get the list of all candidates 
            var candidates = (List<Candidate>)GetCsvRecords();
            if (!candidates.Any())
                return new Response(false, "No record found");

            //if we have records then lets find the specific record
            var candidate = candidates.FirstOrDefault(x => x.Id == id);

            if (candidate is not null)
                candidates.Remove(candidate);

            using StreamWriter sw = new(_path, false, new UTF8Encoding(true));
            using CsvWriter cw = new(sw);
            cw.WriteHeader<Candidate>();
            cw.NextRecord();


            foreach (var item in candidates)
            {
                cw.WriteRecord(item);
                cw.NextRecord();
            }

            //invalidate the cache so as it can fetch new data 
            _cache.Remove(_cacheKey);

            var response = new Response(false, "Record deleted successfully");
            return await Task.FromResult(response);
        }


        /// <summary>
        /// Gets all records of candidates based on specied page and pagesize
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<Candidate>> GetAllAsync(int page, int size)
        {
            //create candidate factory to pass to get or add method of lazy cache
            Func<IEnumerable<Candidate>> candidatesFactory = GetCsvRecords;
            var records = _cache.GetOrAdd(_cacheKey, candidatesFactory);

            var candidates = records.
                Skip((page - 1) * size).
                Take(size).
                ToList();

            return await Task.FromResult(candidates);
        }


        /// <summary>
        /// Gets a candidate by specific candidate id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Candidate> GetAsync(Guid id)
        {
            Func<IEnumerable<Candidate>> candidatesFactory = GetCsvRecords;
            var records = _cache.GetOrAdd(_cacheKey, candidatesFactory);
            var candidate = records.FirstOrDefault(x => x.Id == id);

            if (candidate is null)
                return default;

            return await Task.FromResult(candidate);
        }

        /// <summary>
        /// reads records from csv file
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Candidate> GetCsvRecords()
        {
            try
            {
                using StreamReader reader = new(_path, Encoding.Default);
                using CsvReader csvReader = new(reader);
                csvReader.Configuration.RegisterClassMap<CandidateMap>();
                var records = (csvReader.GetRecords<Candidate>()).ToList();
                return records ?? new List<Candidate>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
