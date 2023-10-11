using Akka.Actor;
using Akka.TestKit.Xunit2;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using static MilleniumFalconChallenge.Actors.ScenarioProcessorsSupervisorActor;

namespace MilleniumFalconChallenge.Actors.Tests
{
    public class ScenarioProcessorSupervisorActorTests : TestKit
    {
        [Theory]
        [AutoData]
        public async void Actor_WhenScenarioSent_AllScenariosProcessed(Scenario scenario1, Scenario scenario2, Scenario scenario3)
        {
            // Arrange
            var runtimeInSeconds = 2;
            Mock<IScenarioRunner> scenarioRunnerMock = new();
            scenarioRunnerMock
                .Setup(r => r.RunAsync(It.IsAny<Scenario>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(runtimeInSeconds));
                    return 100;
                });

            List<Scenario> scenariosProcessed = new();
            Mock<IScenarioRepository> scenarioRepositoryMock = new();
            scenarioRepositoryMock
                .Setup(r => r.UpdateProbabilityAsync(It.IsAny<Scenario>(), It.IsAny<double>()))
                .Callback<Scenario, double>((s, _) =>
                {
                    scenariosProcessed.Add(s);
                })
                .Returns(Task.FromResult(true));
            var sut = Sys.ActorOf(Props.Create(() => new ScenarioProcessorsSupervisorActor(
                2,
                scenarioRunnerMock.Object,
                scenarioRepositoryMock.Object,
                NullLoggerFactory.Instance)));

            // Act
            sut.Tell(new ProcessScenario(scenario1));
            sut.Tell(new ProcessScenario(scenario2));
            sut.Tell(new ProcessScenario(scenario3));

            await Task.Delay(TimeSpan.FromSeconds(runtimeInSeconds / 2));

            // Assert
            Assert.Empty(scenariosProcessed);

            // Assert
            await Task.Delay(TimeSpan.FromSeconds((runtimeInSeconds / 2) + 2));
            Assert.Collection(scenariosProcessed, s1 => s1.Equals(scenario1), s2 => s2.Equals(scenario2));

            // Assert
            await Task.Delay(TimeSpan.FromSeconds(runtimeInSeconds + 2));
            Assert.Collection(scenariosProcessed,
                s1 => s1.Equals(scenario1),
                s2 => s2.Equals(scenario2),
                s3 => s3.Equals(scenario3));
        }
    }
}