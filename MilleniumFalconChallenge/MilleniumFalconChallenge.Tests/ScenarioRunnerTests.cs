using Microsoft.Extensions.Logging.Abstractions;
using MilleniumFalconChallenge.Domain;
using MilleniumFalconChallenge.Domain.Runners;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;
using Xunit;

namespace MilleniumFalconChallenge.Tests
{
    public class ScenarioRunnerTests
    {
        private static readonly Scenario _example1 = new(1, "", 7, null, new BountyHunter[]
        {
            new BountyHunter("Hoth", 6),
            new BountyHunter("Hoth", 7),
            new BountyHunter("Hoth", 8),
        });

        private static readonly Scenario _example2 = new(1, "", 8, null, new BountyHunter[]
        {
            new BountyHunter("Hoth", 6),
            new BountyHunter("Hoth", 7),
            new BountyHunter("Hoth", 8),
        });

        private static readonly Scenario _example3 = new(1, "", 9, null, new BountyHunter[]
        {
            new BountyHunter("Hoth", 6),
            new BountyHunter("Hoth", 7),
            new BountyHunter("Hoth", 8),
        });

        private static readonly Scenario _example4 = new(1, "", 10, null, new BountyHunter[]
        {
            new BountyHunter("Hoth", 6),
            new BountyHunter("Hoth", 7),
            new BountyHunter("Hoth", 8),
        });

        [Fact]
        public async Task RunAsync_Example1_CalculateBestProbability()
        {
            // Arrange
            var routesRepository = new RoutesRepository(new TestsDbContextFactory());
            var loader = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);
            var conf = loader.Load(Path.Combine("Example1", "millenium-falcon.json"));
            var info = new MilleniumFalconInformation(conf!.Autonomy, conf.Departure, conf.Arrival);
            var sut = new ScenarioRunner(routesRepository, info, NullLoggerFactory.Instance);

            // Act
            var probability = await sut.RunAsync(_example1);

            // Assert
            Assert.Equal(0, probability);
        }

        [Fact]
        public async Task RunAsync_Example2_CalculateBestProbability()
        {
            // Arrange
            var routesRepository = new RoutesRepository(new TestsDbContextFactory());
            var loader = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);
            var conf = loader.Load(Path.Combine("Example1", "millenium-falcon.json"));
            var info = new MilleniumFalconInformation(conf!.Autonomy, conf.Departure, conf.Arrival);
            var sut = new ScenarioRunner(routesRepository, info, NullLoggerFactory.Instance);

            // Act
            var probability = await sut.RunAsync(_example2);

            // Assert
            Assert.Equal(0.81, probability);
        }

        [Fact]
        public async Task RunAsync_Example3_CalculateBestProbability()
        {
            // Arrange
            var routesRepository = new RoutesRepository(new TestsDbContextFactory());
            var loader = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);
            var conf = loader.Load(Path.Combine("Example1", "millenium-falcon.json"));
            var info = new MilleniumFalconInformation(conf!.Autonomy, conf.Departure, conf.Arrival);
            var sut = new ScenarioRunner(routesRepository, info, NullLoggerFactory.Instance);

            // Act
            var probability = await sut.RunAsync(_example3);

            // Assert
            Assert.Equal(0.9, probability);
        }

        [Fact]
        public async Task RunAsync_Example4_CalculateBestProbability()
        {
            // Arrange
            var routesRepository = new RoutesRepository(new TestsDbContextFactory());
            var loader = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);
            var conf = loader.Load(Path.Combine("Example1", "millenium-falcon.json"));
            var info = new MilleniumFalconInformation(conf!.Autonomy, conf.Departure, conf.Arrival);
            var sut = new ScenarioRunner(routesRepository, info, NullLoggerFactory.Instance);

            // Act
            var probability = await sut.RunAsync(_example4);

            // Assert
            Assert.Equal(1, probability);
        }
    }
}
