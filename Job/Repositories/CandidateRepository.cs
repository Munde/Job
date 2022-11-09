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
        private readonly string _datastore="candidates.csv";
        private readonly string _path;
        private IWebHostEnvironment _environment;

        public CandidateRepository(IWebHostEnvironment environment)
        {
            _environment = environment;
            _path = $"{_environment}/{_datastore}";
        }

        public async ValueTask<Response> CreateOrUpdateAsync(Candidate candidate)
        {
            try
            {
                //create a stream write by providing path and the encoding type 
                using StreamWriter sw=new(_path, false,new UTF8Encoding(true));
                using CsvWriter cw = new(sw);

                //Write header of the csv using the candidate fields then move to next record
                cw.WriteHeader<Candidate>();
                cw.NextRecord();

                //write the candidate details to the csv
                cw.WriteRecord(candidate);
                cw.NextRecord();

                return await Task.FromResult(new Response(true, "Candidate details recorded successfully"));
            }
            catch (Exception ex)
            {
                return new Response(false, ex.Message);
            }
        }

        public ValueTask<Response> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<IEnumerable<Candidate>> GetAllAsync(int page, int size)
        {
            try
            {
                using StreamReader reader = new(_path, Encoding.Default);
                using CsvReader csvReader = new CsvReader(reader);
                csvReader.Configuration.RegisterClassMap<CandidateMap>();
                var records = csvReader.GetRecords<Candidate>();
                return await Task.FromResult(records.Skip((page-1)*size).Take(size).ToList());
            }catch(Exception)
            {
                throw;
            }
        }

        public async ValueTask<Candidate> GetAsync(Guid id)
        {
            try
            {
                using StreamReader reader = new(_path, Encoding.Default);
                using CsvReader csvReader = new CsvReader(reader);
                csvReader.Configuration.RegisterClassMap<CandidateMap>();
                var records = csvReader.GetRecords<Candidate>();
                var candidate = records.FirstOrDefault(x => x.Id == id);
                Candidate? result = candidate ?? default;
                return await Task.FromResult<Candidate>(result);
            }catch(Exception)
            {
                throw;
            }
        }
    }
}
