using AutoFixture.Xunit2;
using Moq;
using Xunit;
using static MFC.Domain.INewScenarioHandler;

namespace MFC.Domain.Tests
{
    public class NewScenarioHandlerTests
    {
        [Theory]
        [AutoData]
        public async Task HandleAsync_InvalidPlanets_ReturnsInvalidResult(NewScenario newScenario)
        {
            // Arrange
            var dispatcherMock = new Mock<IScenarioProcessingDispatcher>();
            var scenarioRepository = new Mock<IScenarioRepository>();
            var routesRepository = new Mock<IReadOnlyRoutesRepository>();
            routesRepository.Setup(m => m.DoesPlanetExistsAsync(It.IsAny<PlanetIdentifier>()))
                .Returns(Task.FromResult(false));

            var sut = new NewScenarioHandler(
                dispatcherMock.Object,
                scenarioRepository.Object,
                routesRepository.Object);

            // Act
            var result = await sut.HandleAsync(newScenario);

            // Assert
            Assert.IsType<InvalidScenario>(result);
        }

        [Theory]
        [AutoData]
        public async Task HandleAsync_ValidPlanets_ReturnsSuccessResult(NewScenario newScenario, Scenario scenario)
        {
            // Arrange
            var dispatcherMock = new Mock<IScenarioProcessingDispatcher>();
            var scenarioRepository = new Mock<IScenarioRepository>();
            scenarioRepository.Setup(m => m.InsertAsync(It.IsAny<NewScenario>()))
                .Returns(Task.FromResult<Scenario?>(scenario));
            var routesRepository = new Mock<IReadOnlyRoutesRepository>();
            routesRepository.Setup(m => m.DoesPlanetExistsAsync(It.IsAny<PlanetIdentifier>()))
                .Returns(Task.FromResult(true));

            var sut = new NewScenarioHandler(
                dispatcherMock.Object,
                scenarioRepository.Object,
                routesRepository.Object);

            // Act
            var result = await sut.HandleAsync(newScenario);

            // Assert
            Assert.IsType<Success>(result);
        }

        [Theory]
        [AutoData]
        public async Task HandleAsync_UnexpectedErrorDuringInsert_ReturnsUnexpectedErrorResult(NewScenario newScenario)
        {
            // Arrange
            var dispatcherMock = new Mock<IScenarioProcessingDispatcher>();
            var scenarioRepository = new Mock<IScenarioRepository>();
            scenarioRepository.Setup(m => m.InsertAsync(It.IsAny<NewScenario>()))
                .Returns(Task.FromResult<Scenario?>(null));
            var routesRepository = new Mock<IReadOnlyRoutesRepository>();
            routesRepository.Setup(m => m.DoesPlanetExistsAsync(It.IsAny<PlanetIdentifier>()))
                .Returns(Task.FromResult(true));

            var sut = new NewScenarioHandler(
                dispatcherMock.Object,
                scenarioRepository.Object,
                routesRepository.Object);

            // Act
            var result = await sut.HandleAsync(newScenario);

            // Assert
            Assert.IsType<UnexpectedError>(result);
        }
    }
}