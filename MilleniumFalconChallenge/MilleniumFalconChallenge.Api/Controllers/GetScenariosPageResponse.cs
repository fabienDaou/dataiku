namespace MilleniumFalconChallenge.Api.Controllers
{
    public record GetScenariosPageResponse(int Total, GetScenariosPageResponse.Scenario[] Scenarios)
    {
        public record Scenario(int Id, string Name, int Countdown, double? Probability, BountyHunter[] BountyHunters)
        {
            public Scenario(MilleniumFalconChallenge.Scenario scenario) : this(
                scenario.Id,
                scenario.Name,
                scenario.Countdown,
                scenario.Probability,
                scenario.BountyHunters.Select(b => new BountyHunter(b.Planet, b.Day)).ToArray())
            {
            }

        }
        public record BountyHunter(string Planet, int Day);
    }
}