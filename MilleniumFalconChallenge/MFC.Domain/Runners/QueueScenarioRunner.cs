using Microsoft.Extensions.Logging;

namespace MFC.Domain.Runners
{
    public class QueueScenarioRunner : IScenarioRunner
    {
        private readonly IReadOnlyRoutesRepository _routesRepository;
        private readonly MilleniumFalconInformation _milleniumFalconInformation;
        private readonly ILogger _logger;

        public QueueScenarioRunner(
            IReadOnlyRoutesRepository routesRepository,
            MilleniumFalconInformation milleniumFalconInformation,
            ILoggerFactory loggerFactory)
        {
            _routesRepository = routesRepository ?? throw new ArgumentNullException(nameof(routesRepository));
            _milleniumFalconInformation = milleniumFalconInformation ?? throw new ArgumentNullException(nameof(milleniumFalconInformation));
            _logger = loggerFactory?.CreateLogger<HashSetScenarioRunner>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<double> RunAsync(Scenario scenario)
        {
            var allRoutes = await _routesRepository.GetAllRoutesAsync();
            Dictionary<PlanetIdentifier, HashSet<Edge>> planetToEdgesDictionary = new();
            foreach (var route in allRoutes)
            {
                var (origin, destination, travelTime) = route;
                Edge newEdge = new(destination, travelTime);
                if (planetToEdgesDictionary.TryGetValue(origin, out var edgeSet))
                {
                    edgeSet.Add(newEdge);
                }
                else
                {
                    planetToEdgesDictionary.Add(origin, new HashSet<Edge> { newEdge });
                }
            }

            var countdown = scenario.Countdown;
            var (maxAutonomy, departure, arrival) = _milleniumFalconInformation;

            List<Itinerary> possibleSolutions = new();

            Queue<Itinerary> itineraries = new();
            itineraries.Enqueue(new(maxAutonomy, countdown, departure, 0));

            var maxQueueSize = 0;
            var numberOfLoops = 0;
            do
            {
                numberOfLoops++;
                if (maxQueueSize < itineraries.Count)
                {
                    maxQueueSize = itineraries.Count;
                }

                var itinerary = itineraries.Dequeue();

                // Are we on a planet with bounty hunters?
                var bountyHunterEncounter = false;
                foreach (var bountyHunter in scenario.BountyHunters)
                {
                    if (bountyHunter.Planet == itinerary.CurrentPlanet
                        && countdown - itinerary.DaysLeft == bountyHunter.Day)
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

                if (!planetToEdgesDictionary.TryGetValue(itinerary.CurrentPlanet, out var edges))
                {
                    throw new Exception($"Unexpected planet '{itinerary.CurrentPlanet}'.");
                }

                foreach (var edge in edges)
                {
                    if (edge.TravelTime > itinerary.AutonomyLeft)
                    {
                        // Not enough autonomy to reach next planet
                        continue;
                    }

                    PlanetIdentifier nextPlanet = edge.Identifier;
                    var moveToPlanetItinerary = DeepCopy(itinerary);
                    moveToPlanetItinerary.CurrentPlanet = nextPlanet;
                    moveToPlanetItinerary.DaysLeft -= edge.TravelTime;
                    moveToPlanetItinerary.AutonomyLeft -= edge.TravelTime;
                    if (bountyHunterEncounter)
                    {
                        moveToPlanetItinerary.BountyHunterEncounters++;
                    }

                    itineraries.Enqueue(moveToPlanetItinerary);
                }
            }
            while (itineraries.Count > 0);

            _logger.LogInformation($"LoopsNumber({numberOfLoops})Max queue size ({maxQueueSize}).");

            return possibleSolutions.Any()
                ? possibleSolutions
                    .Select(s => CalculateSuccessProbability(s.BountyHunterEncounters))
                    .Max()
                : 0;
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
                failureProbability = failureProbability + Math.Pow(9, i - 1) / Math.Pow(10, i);
            }

            return 1 - failureProbability;
        }
    }
}
