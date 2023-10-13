using Microsoft.Extensions.Logging;

namespace MilleniumFalconChallenge.Domain.Runners
{
    public class ScenarioRunner : IScenarioRunner
    {
        private readonly IReadOnlyRoutesRepository _routesRepository;
        private readonly MilleniumFalconInformation _milleniumFalconInformation;
        private readonly ILogger _logger;

        public ScenarioRunner(
            IReadOnlyRoutesRepository routesRepository,
            MilleniumFalconInformation milleniumFalconInformation,
            ILoggerFactory loggerFactory)
        {
            _routesRepository = routesRepository ?? throw new ArgumentNullException(nameof(routesRepository));
            _milleniumFalconInformation = milleniumFalconInformation ?? throw new ArgumentNullException(nameof(milleniumFalconInformation));
            _logger = loggerFactory?.CreateLogger<ScenarioRunner>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<double> RunAsync(Scenario scenario)
        {
            var countdown = scenario.Countdown;
            var (maxAutonomy, departure, arrival) = _milleniumFalconInformation;

            List<Itinerary> possibleSolutions = new();

            Queue<Itinerary> itineraries = new();
            itineraries.Enqueue(new(maxAutonomy, countdown, departure, 0));

            do
            {
                var itinerary = itineraries.Dequeue();

                // Are we on a planet with bounty hunters?
                var bountyHunterEncounter = false;
                foreach (var bountyHunter in scenario.BountyHunters)
                {
                    if (bountyHunter.Planet == itinerary.CurrentPlanet
                        && (countdown - itinerary.DaysLeft) == bountyHunter.Day)
                    {
                        bountyHunterEncounter = true;
                        break;
                    }
                }

                // Late, it is not a possible solution.
                if (itinerary.DaysLeft < 0)
                {
                    continue;
                }

                // Arrived on time, it is a possible solution.
                if (itinerary.CurrentPlanet == arrival)
                {
                    possibleSolutions.Add(itinerary);
                    continue;
                }

                // StayPut: one day spent on the same planet
                var stayPutItinerary = DeepCopy(itinerary);
                stayPutItinerary.DaysLeft--;
                if (bountyHunterEncounter)
                {
                    stayPutItinerary.BountyHunterEncounters++;
                }
                itineraries.Enqueue(stayPutItinerary);

                // StayPutAndRefuel: one day spent on the same planet and refuel
                var stayPutAndRefuelItinerary = DeepCopy(itinerary);
                stayPutAndRefuelItinerary.DaysLeft--;
                stayPutAndRefuelItinerary.AutonomyLeft = maxAutonomy;
                if (bountyHunterEncounter)
                {
                    stayPutAndRefuelItinerary.BountyHunterEncounters++;
                }
                itineraries.Enqueue(stayPutAndRefuelItinerary);

                var routesFromThisPlanet = await _routesRepository.GetRoutesAsync(itinerary.CurrentPlanet);
                foreach (var route in routesFromThisPlanet)
                {
                    if (route.TravelTime > itinerary.AutonomyLeft)
                    {
                        // Not enough autonomy to reach next planet
                        continue;
                    }

                    PlanetIdentifier nextPlanet = itinerary.CurrentPlanet == route.Origin ? route.Destination : route.Origin;
                    var moveToPlanetItinerary = DeepCopy(itinerary);
                    moveToPlanetItinerary.CurrentPlanet = nextPlanet;
                    moveToPlanetItinerary.DaysLeft -= route.TravelTime;
                    moveToPlanetItinerary.AutonomyLeft -= route.TravelTime;
                    if (bountyHunterEncounter)
                    {
                        moveToPlanetItinerary.BountyHunterEncounters++;
                    }

                    itineraries.Enqueue(moveToPlanetItinerary);
                }
            }
            while (itineraries.Count > 0);

            if (possibleSolutions.Any())
            {
                return possibleSolutions.Select(s =>
                {
                    return CalculateSuccessProbability(s.BountyHunterEncounters);
                }).Max();
            }
            else
            {
                return 0;
            }
        }

        private static Itinerary DeepCopy(Itinerary original)
        {
            return new(
                original.AutonomyLeft,
                original.DaysLeft,
                original.CurrentPlanet,
                original.BountyHunterEncounters);
        }

        private static double CalculateSuccessProbability(int encounters)
        {
            if (encounters == 0)
            {
                return 1;
            }

            double failureProbability = 0;
            for (int i = 1; i <= encounters; i++)
            {
                failureProbability = failureProbability + (Math.Pow(9, i - 1) / Math.Pow(10, i));
            }

            return 1 - failureProbability;
        }

        private interface IAction { }
        private record StayPut(PlanetIdentifier Planet) : IAction;
        private record StayPutAndRefuel(PlanetIdentifier Planet) : IAction;
        private record MoveTo(PlanetIdentifier Planet, int TravelTime) : IAction;

        private record Itinerary
        {
            public int AutonomyLeft { get; set; }
            public int DaysLeft { get; set; }
            public int DaysPassed => 8 - DaysLeft;
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
}
