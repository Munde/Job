using CsvHelper.Configuration;

using Job.Shared.Models.Candidates;

namespace Job.Mappers
{
    public class CandidateMap:ClassMap<Candidate>
    {
        public CandidateMap()
        {
            Map(x => x.Id).Name("Id");
            Map(x => x.Firstname).Name("Firstname");
            Map(x => x.Lastname).Name("Lastname");
            Map(x => x.Email).Name("Email");
            Map(x => x.Phone).Name("Phone");
            Map(x => x.CallTimeInterval).Name("CallTimeInterval");
            Map(x => x.LinkedInProfile).Name("LinkedInProfile");
            Map(x => x.GithubLink).Name("GithubLink");
            Map(x => x.Comment).Name("Comment");
            Map(x => x.CreatedAt).Name("CreatedAt");
        }
    }
}
