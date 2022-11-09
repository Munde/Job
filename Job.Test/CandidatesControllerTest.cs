using FluentAssertions;

using Job.Controllers;
using Job.Domains.Candidates;
using Job.Shared.Models.Candidates;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Job.Test
{
    public class CandidatesControllerTest
    {
        [Fact]
        public async Task GetCandidates_WithEmptyList_ReturnNotFound()
        {
            //Arrange
            int page = 1,
                size = 5;
            var repositoryStub = new Mock<ICandidateRepository>();
            repositoryStub.Setup(repo => repo.GetAllAsync(page, size))
                .ReturnsAsync(new List<Candidate>());

            var controller = new CandidatesController(repositoryStub.Object);
            //Act
            var actionResult = await controller.Get(page, size);

            //Assert
            var result = actionResult.Result;
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCandidates_FromListOfExistingCandidates_ReturnsCandidates()
        {
            //Arrange
            int page = 1,
                size = 5;
            var candidates = new List<Candidate>()
            {
                RandomCandidate(),
                RandomCandidate(),
                RandomCandidate(),
                RandomCandidate(),
                RandomCandidate(),
                RandomCandidate(),
                RandomCandidate()
            };

            var repositoryStub = new Mock<ICandidateRepository>();
            repositoryStub.Setup(repo => repo.GetAllAsync(page, size))
                .ReturnsAsync(candidates);

            var controller = new CandidatesController(repositoryStub.Object);

            //Act
            var actionResult = await controller.Get(page, size);

            //ASSERT
            var result = actionResult.Result as OkObjectResult;
            var returnedCandidates = result.Value as List<Candidate>;
            returnedCandidates.Should().BeEquivalentTo(candidates, 
                option=>option.ComparingByMembers<Candidate>());
        }

        private Candidate RandomCandidate() => new Candidate
        {
            Id = Guid.NewGuid(),
            Firstname = Guid.NewGuid().ToString(),
            Lastname = Guid.NewGuid().ToString(),
            Phone = (new Random()).Next(999999999).ToString(),
            Email = $"{Guid.NewGuid()}@example.com",
            CallTimeInterval = Guid.NewGuid().ToString(),
            Comment = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            GithubLink = $"https://github.com/{Guid.NewGuid()}",
            LinkedInProfile = $"https://linkedin.com/in/{Guid.NewGuid()}"
        };
    }
}