using Akka.Actor;

namespace MilleniumFalconChallenge.Actors
{
    public class NewScenarioHandler : INewScenarioHandler
    {
        private readonly IActorRef _processorSupervisorRef;
        private readonly IScenarioRepository _scenarioRepository;

        public NewScenarioHandler(IActorRef processorSupervisorRef, IScenarioRepository scenarioRepository)
        {
            _processorSupervisorRef = processorSupervisorRef ?? throw new ArgumentNullException(nameof(processorSupervisorRef));
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
        }

        public async Task<int?> HandleAsync(NewScenario scenario)
        {
            var scenarioCreated = await _scenarioRepository.InsertAsync(scenario);
            if (scenarioCreated is not null)
            {
                _processorSupervisorRef.Tell(new ScenarioProcessorsSupervisorActor.ProcessScenario(scenarioCreated));

                return scenarioCreated.Id;
            }
            else
            {
                return null;
            }
        }
    }
}