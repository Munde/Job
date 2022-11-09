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
    }
}