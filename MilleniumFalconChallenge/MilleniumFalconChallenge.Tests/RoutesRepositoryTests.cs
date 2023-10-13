using Microsoft.EntityFrameworkCore;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;
using Xunit;

namespace MilleniumFalconChallenge.Tests
{
    public class RoutesRepositoryTests
    {
        [Theory]
        [InlineData("Tatooine", true)]
        [InlineData("Dagobah", true)]
        [InlineData("Endor", true)]
        [InlineData("Hoth", true)]
        [InlineData("earth", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task DoesPlanetExistAsync_Exists_ReturnTrue(string planet, bool exists)
        {
            // Arrange
            var sut = new RoutesRepository(new DbContextFactory());

            // Act
            var result = await sut.DoesPlanetExistsAsync(planet);

            // Assert
            Assert.Equal(exists, result);
        }

        [Fact]
        public async Task GetRoutesAsync_WhenCalled_ReturnsAllRoutes()
        {
            // Arrange
            var sut = new RoutesRepository(new DbContextFactory());

            // Act
            var result = await sut.GetRoutesAsync("Tatooine");

            // Assert
            Assert.Collection(result,
                r1 => Assert.Equal(new Route("Tatooine", "Dagobah", 6), r1),
                r2 => Assert.Equal(new Route("Tatooine", "Hoth", 6), r2));
        }

        private class DbContextFactory : IDbContextFactory<RoutesDbContext>
        {
            public RoutesDbContext CreateDbContext()
            {
                var options = new DbContextOptionsBuilder<RoutesDbContext>()
                    .UseSqlite("Data Source=universe.db")
                    .Options;
                return new RoutesDbContext(options);
            }
        }
    }
}