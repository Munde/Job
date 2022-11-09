using FluentAssertions;

using Job.Controllers;
using Job.Domains.Candidates;
using Job.Shared.Dtos.Candidates;
using Job.Shared.Models.Candidates;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Job.Test
{
    public class CandidatesControllerTest
    {
        private readonly new Mock<ICandidateRepository> repositoryStub=new();
        [Fact]
        public async Task GetCandidates_WithEmptyList_ReturnNotFound()
        {
            //Arrange
            int page = 1,
                size = 5;

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


        [Fact]
        public async Task PostCandidate_WithCandidateToBePosted_ReturnsCreatedCandidate()
        {
            //Arrange
            var candidateToBePosted = new CandidateDto
            {
                Email = "zmunde5@gmail.com",
                Firstname = "Zephania",
                Lastname = "Mundekesye",
                Phone = "+255785900162",
                CallTimeInterval = "any time from 2pm to 6pm",
                Comment = "Hard working individual ready to learn",
                GithubLink = "https://github.com/munde",
                LinkedInProfile = "https://www.linkedin.com/in/zephania-eliah-870b834b/",
            };

            var controller = new CandidatesController(repositoryStub.Object);

            //Act
            var createdActionResult = await controller.Post(candidateToBePosted);

            //Asset
            var result = createdActionResult.Result as CreatedAtActionResult;
            var createdCandidate = result.Value as Candidate;
            createdCandidate.Should().NotBeEquivalentTo(candidateToBePosted,
                options=>options.ComparingByMembers<CandidateDto>().
                ExcludingMissingMembers());
        }


        [Fact]
        public async Task DeleteCandidate_WithExistingCandidate_ReturnsNoConent()
        {
            //Arrange
            var existingCandidate = RandomCandidate();
            repositoryStub.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(existingCandidate);

            var controller = new CandidatesController(repositoryStub.Object);
            //Act

            var result = await controller.Delete(existingCandidate.Id);

            //Assert
            result.Result.Should().BeOfType<NoContentResult>();
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