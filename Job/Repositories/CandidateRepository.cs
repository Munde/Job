using CsvHelper;

using Job.Domains.Candidates;
using Job.Mappers;
using Job.Shared;
using Job.Shared.Models.Candidates;

using System.IO;
using System.Text;

namespace Job.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly string _datastore = "candidates.csv";
        private readonly string _path;
        private IWebHostEnvironment _environment;

        public CandidateRepository(IWebHostEnvironment environment)
        {
            _environment = environment;
            _path = $"{_environment.WebRootPath}/{_datastore}";
        }

        public async ValueTask<Response> CreateOrUpdateAsync(Candidate candidate)
        {
            try
            {

                //Get all the records and cast to list 
                List<Candidate> candidates = (List<Candidate>)GetCsvRecords();

                //check if the candidate details exists using given email
                var existingCandidate = candidates.FirstOrDefault(x=>x.Email == candidate.Email);

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

                return await Task.FromResult(new Response(true, "Candidate details recorded successfully"));
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }

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

            return await Task.FromResult(new Response(false, "Record deleted successfully"));
        }

        public async ValueTask<IEnumerable<Candidate>> GetAllAsync(int page, int size)
        {
            var records = GetCsvRecords();
            return await Task.FromResult(records.Skip((page - 1) * size).Take(size).ToList());
        }

        public async ValueTask<Candidate> GetAsync(Guid id)
        {
            var records = GetCsvRecords();
            var candidate = records.FirstOrDefault(x => x.Id == id);
            Candidate? result = candidate ?? default;
            return await Task.FromResult<Candidate>(result);
        }


        private IEnumerable<Candidate> GetCsvRecords()
        {
            try
            {
                using StreamReader reader = new(_path, Encoding.Default);
                using CsvReader csvReader = new(reader);
                csvReader.Configuration.RegisterClassMap<CandidateMap>();
                var records = (csvReader.GetRecords<Candidate>()).ToList();
                return records??new List<Candidate>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
