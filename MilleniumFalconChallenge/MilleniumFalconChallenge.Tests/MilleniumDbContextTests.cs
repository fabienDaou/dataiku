using MilleniumFalconChallenge.Persistence.MilleniumFalcon;
using Xunit;

namespace MilleniumFalconChallenge.Tests
{
    public class MilleniumDbContextTests
    {
        [Fact]
        public void MilleniumDbContext_WhenRoutesQueried_RoutesProperlyMapped()
        {
            // Arrange
            var sut = new MilleniumDbContext("Data Source=universe.db");

            // Act
            var routes = sut.Routes.ToList();

            // Assert
            Assert.Equal(5, routes.Count);
            Assert.Collection(routes,
                r1 =>
                {
                    Assert.Equal("Tatooine", r1.Origin);
                    Assert.Equal("Dagobah", r1.Destination);
                    Assert.Equal(6, r1.TravelTime);
                },
                r2 =>
                {
                    Assert.Equal("Dagobah", r2.Origin);
                    Assert.Equal("Endor", r2.Destination);
                    Assert.Equal(4, r2.TravelTime);
                },
                r3 =>
                {
                    Assert.Equal("Dagobah", r3.Origin);
                    Assert.Equal("Hoth", r3.Destination);
                    Assert.Equal(1, r3.TravelTime);
                },
                r4 =>
                {
                    Assert.Equal("Hoth", r4.Origin);
                    Assert.Equal("Endor", r4.Destination);
                    Assert.Equal(1, r4.TravelTime);
                },
                r5 =>
                {
                    Assert.Equal("Tatooine", r5.Origin);
                    Assert.Equal("Hoth", r5.Destination);
                    Assert.Equal(6, r5.TravelTime);
                });
        }
    }
}