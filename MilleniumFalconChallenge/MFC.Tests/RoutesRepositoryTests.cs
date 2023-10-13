using MFC.Domain;
using MFC.Persistence.MilleniumFalcon;
using Xunit;

namespace MFC.Tests
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
            var sut = new RoutesRepository(new RoutesDbContextFactory("universe.db"));

            // Act
            var result = await sut.DoesPlanetExistsAsync(planet);

            // Assert
            Assert.Equal(exists, result);
        }

        [Fact]
        public async Task GetRoutesAsync_WhenCalled_ReturnsAllRoutes()
        {
            // Arrange
            var sut = new RoutesRepository(new RoutesDbContextFactory("universe.db"));

            // Act
            var result = await sut.GetRoutesAsync("Tatooine");

            // Assert
            Assert.Collection(result,
                r1 => Assert.Equal(new Route("Tatooine", "Dagobah", 6), r1),
                r2 => Assert.Equal(new Route("Tatooine", "Hoth", 6), r2));
        }
    }
}