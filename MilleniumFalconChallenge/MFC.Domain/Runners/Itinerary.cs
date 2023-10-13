namespace MFC.Domain.Runners
{
    public record Itinerary
    {
        public int AutonomyLeft { get; set; }
        public int DaysLeft { get; set; }
        public PlanetIdentifier CurrentPlanet { get; set; }
        public int BountyHunterEncounters { get; set; }

        public Itinerary(
            int autonomyLeft,
            int daysLeft,
            PlanetIdentifier currentPlanet,
            int bountyHunterEncounters)
        {
            AutonomyLeft = autonomyLeft;
            DaysLeft = daysLeft;
            CurrentPlanet = currentPlanet;
            BountyHunterEncounters = bountyHunterEncounters;
        }
    }
}
