using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging.Abstractions;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;
using Xunit;

namespace MilleniumFalconChallenge.Tests
{
    public class MilleniumFalconConfigurationLoaderTests
    {
        [Theory]
        [AutoData]
        public async Task Load_ValidJson_PropertiesWellSet(Guid guid)
        {
            // Arrange
            var configuration = @"
                {
                    ""autonomy"": 6,
                    ""departure"": ""Tatooine"",
                    ""arrival"": ""Endor"",
                    ""routes_db"": ""universe.db""
                }";

            var path = Path.Combine(Environment.CurrentDirectory, guid.ToString());

            try
            {
                using (var sw = new StreamWriter(path, true))
                {
                    await sw.WriteAsync(configuration);
                }

                var sut = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);

                // Act
                var result = sut.Load(path);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(6, result.Autonomy);
                Assert.Equal("Tatooine", result.Departure);
                Assert.Equal("Endor", result.Arrival);
                Assert.Equal("universe.db", result.RoutesDbPath);
            }
            finally
            {
                // Cleanup
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Theory]
        [AutoData]
        public void Load_UnknownPath_ReturnsNull(Guid guid)
        {
            // Arrange
            var path = Path.Combine(Environment.CurrentDirectory, guid.ToString());

            var sut = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);

            // Act
            var result = sut.Load(path);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [AutoData]
        public async Task Load_RelativeToCurrentDirectory_PropertiesWellSet(Guid guid)
        {
            // Arrange
            var configuration = @"
                {
                    ""autonomy"": 6,
                    ""departure"": ""Tatooine"",
                    ""arrival"": ""Endor"",
                    ""routes_db"": ""universe.db""
                }"
            ;

            var path = Path.Combine(Environment.CurrentDirectory, guid.ToString());

            try
            {
                using (var sw = new StreamWriter(path, true))
                {
                    await sw.WriteAsync(configuration);
                }

                var sut = new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance);

                // Act
                var result = sut.Load(Path.GetRelativePath(Environment.CurrentDirectory, path));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(6, result.Autonomy);
                Assert.Equal("Tatooine", result.Departure);
                Assert.Equal("Endor", result.Arrival);
                Assert.Equal("universe.db", result.RoutesDbPath);
            }
            finally
            {
                // Cleanup
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}