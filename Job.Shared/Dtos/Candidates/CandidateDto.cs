using System.ComponentModel.DataAnnotations;

namespace Job.Shared.Dtos.Candidates
{
    public class CandidateDto
    {
        [StringLength(50)]
        public required string Firstname { get; set; }
        [StringLength(50)]
        public required string Lastname { get; set; }

        [StringLength(50), EmailAddress]
        public required string Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? CallTimeInterval { get; set; }

        [StringLength(100),Url(ErrorMessage ="Provide a valid linked in profile link")]

        public string? LinkedInProfile { get; set; }

        [StringLength(100),Url(ErrorMessage ="Provide a valid github link")]
        public string? GithubLink { get; set; }
        public string? Comment { get; set; }
    }
}
