using Akka.Actor;
using Akka.TestKit.Xunit2;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using static MilleniumFalconChallenge.Actors.ScenarioProcessorActor;
using Status = MilleniumFalconChallenge.Actors.ScenarioProcessorActor.Status;

namespace MilleniumFalconChallenge.Actors.Tests
{
    public class ScenarioProcessorActorTests : TestKit
    {
        [Fact]
        public async Task Actor_WhenStarted_IsAvailable()
        {
            // Arrange
            var sut = Sys.ActorOf(Props.Create(() => new ScenarioProcessorActor(
                new Mock<IScenarioRunner>().Object,
                new Mock<IScenarioRepository>().Object,
                NullLoggerFactory.Instance)));

            // Act
            var response = await sut.Ask<GetStatusResponse>(new GetStatusQuery(), TimeSpan.FromSeconds(1));

            // Assert
            Assert.Equal(Status.Available, response.Status);
        }

        [Theory]
        [AutoData]
        public async Task Actor_WhenScenarioProcessing_IsBusy(Scenario scenario)
        {
            // Arrange
            var scenarioRunnerMock = new Mock<IScenarioRunner>();
            scenarioRunnerMock
                .Setup(r => r.RunAsync(It.IsAny<Scenario>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return 100;
                });
            var sut = Sys.ActorOf(Props.Create(() => new ScenarioProcessorActor(
                scenarioRunnerMock.Object,
                new Mock<IScenarioRepository>().Object,
                NullLoggerFactory.Instance)));

            // Act
            sut.Tell(new StartProcessCommand(scenario));

            await Task.Delay(TimeSpan.FromSeconds(3));

            // Assert
            var response = await sut.Ask<GetStatusResponse>(new GetStatusQuery(), TimeSpan.FromSeconds(1));
            Assert.Equal(Status.Busy, response.Status);

            // Act
            await Task.Delay(TimeSpan.FromSeconds(3));
            response = await sut.Ask<GetStatusResponse>(new GetStatusQuery(), TimeSpan.FromSeconds(1));
            Assert.Equal(Status.Available, response.Status);
        }
    }
}