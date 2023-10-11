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

        private class DbContextFactory : IDbContextFactory<MilleniumDbContext>
        {
            public MilleniumDbContext CreateDbContext()
            {
                var options = new DbContextOptionsBuilder<MilleniumDbContext>()
                    .UseSqlite("Data Source=universe.db")
                    .Options;
                return new MilleniumDbContext(options);
            }
        }
    }
}