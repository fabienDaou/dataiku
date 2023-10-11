using Akka.Actor;
using Microsoft.Extensions.Logging;
using static MilleniumFalconChallenge.Actors.ScenarioProcessorActor;

namespace MilleniumFalconChallenge.Actors
{
    /// <summary>
    /// Manages a pool of processors and keep track of scenarios to process.
    /// </summary>
    public class ScenarioProcessorsSupervisorActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private List<IActorRef> _processorRefs = new();
        private readonly IScenarioRunner _runner;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly Queue<Scenario> _scenariosQueue = new();

        public ScenarioProcessorsSupervisorActor(
            int processNumber,
            IScenarioRunner runner,
            IScenarioRepository scenarioRepository,
            ILoggerFactory loggerFactory)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<ScenarioProcessorsSupervisorActor>();

            Become(Starting);

            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.Zero, Self, new StartProcessors(processNumber), Self);
        }

        private void Starting()
        {
            Receive<StartProcessors>(m =>
            {
                for (var i = 0; i < m.ProcessorNumber; i++)
                {
                    var actorRef = Context.ActorOf(Props.Create(() => new ScenarioProcessorActor(
                        _runner,
                        _scenarioRepository,
                        _loggerFactory)), $"scenario-processor-{i}");

                    _processorRefs.Add(actorRef);
                }

                Become(Started);

                Stash.UnstashAll();
            });
            Receive<object>(m => Stash.Stash());
        }

        private void Started()
        {
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(1),
                Self,
                new CheckScenariosToProcess(),
                Self);

            ReceiveAsync<ProcessScenario>(async m =>
            {
                for (var i = 0; i < _processorRefs.Count; i++)
                {
                    var processorRef = _processorRefs[i];
                    var response = await processorRef.Ask<StartProcessResponse>(new StartProcessCommand(m.Scenario), TimeSpan.FromSeconds(2));
                    if (response.Accepted)
                    {
                        _logger.LogInformation("Scenario '{Name}' sent for processing.", m.Scenario.Name);
                        return;
                    }
                }

                _scenariosQueue.Enqueue(m.Scenario);
            });

            ReceiveAsync<CheckScenariosToProcess>(async m =>
            {
                for (var i = 0; i < _processorRefs.Count; i++)
                {
                    if (_scenariosQueue.Count <= 0)
                    {
                        break;
                    }

                    var processorRef = _processorRefs[i];
                    var scenario = _scenariosQueue.Peek();
                    var response = await processorRef.Ask<StartProcessResponse>(new StartProcessCommand(scenario));
                    if (response.Accepted)
                    {
                        _scenariosQueue.Dequeue();
                        _logger.LogInformation("Scenario '{Name}' sent for processing.", scenario.Name);
                    }
                }

                Context.System.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromSeconds(1),
                    Self,
                    new CheckScenariosToProcess(),
                    Self);
            });
        }

        private record StartProcessors(int ProcessorNumber);
        private record CheckScenariosToProcess();

        public record ProcessScenario(Scenario Scenario);
    }
}