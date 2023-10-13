namespace MFC.Domain
{
    public record MilleniumFalcon(int Autonomy, PlanetIdentifier Departure, PlanetIdentifier Arrival);

    public record PlanetIdentifier(string Name)
    {
        public static implicit operator string(PlanetIdentifier d) => d.Name;
        public static implicit operator PlanetIdentifier(string d) => new(d);
    }

    public record Planet(PlanetIdentifier Identifier, Route[] Routes);

    public record Route(PlanetIdentifier Origin, PlanetIdentifier Destination, int TravelTime);

    public record Empire(int Countdown, BountyHunter[] BountyHunters);

    public record BountyHunter(PlanetIdentifier Planet, int Day);

    public record Scenario(int Id, string Name, int Countdown, double? Probability, BountyHunter[] BountyHunters);

    public record NewScenario(string Name, int Countdown, BountyHunter[] BountyHunters);
}
