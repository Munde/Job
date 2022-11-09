using System.ComponentModel.DataAnnotations;

namespace Job.Shared.Models.Candidates
{
    public class Candidate :BaseEntity
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

        [StringLength(100)]
        public string? LinkedInProfile { get; set; }

        [StringLength(100)]
        public string? GithubLink { get; set; }
        public string? Comment { get; set; }
    }
}
