using Akka.Actor;
using MFC.Domain;
using Microsoft.Extensions.Logging;

namespace MFC.Actors
{
    /// <summary>
    /// Actor hosting a scenario processing.
    /// </summary>
    public class ScenarioProcessorActor : ReceiveActor
    {
        private readonly ILogger _logger;
        private readonly IScenarioRunner _runner;
        private readonly IScenarioRepository _scenarioRepository;

        private CancellationTokenSource? _processingCts;

        public ScenarioProcessorActor(
            IScenarioRunner runner,
            IScenarioRepository scenarioRepository,
            ILoggerFactory loggerFactory)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
            _logger = loggerFactory?.CreateLogger<ScenarioProcessorActor>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            Become(Available);
        }

        #region Available
        private void Available()
        {
            Receive<StartProcessCommand>(Handle);
            Receive<GetStatusQuery>(_ =>
            {
                Sender.Tell(new GetStatusResponse(Status.Available));
                return true;
            });
        }

        private bool Handle(StartProcessCommand c)
        {
            Become(Busy);
            _logger.LogInformation("Start processing scenario '{Name}'.", c.Scenario.Name);

            Sender.Tell(new StartProcessResponse(true));

            _processingCts = new CancellationTokenSource();

            // closure
            var sender = Sender;
            var self = Self;

            Task.Run(async () =>
            {
                return await _runner.RunAsync(c.Scenario);
            }, _processingCts.Token).ContinueWith(async previousTask =>
            {
                if (previousTask.IsFaulted)
                {
                    _logger.LogError("An error occured during scenario '{Name}' processing.", c.Scenario.Name);
                }
                else if (previousTask.IsCanceled)
                {
                    _logger.LogError("Scenario '{Name}' processing was cancelled.", c.Scenario.Name);
                }
                else
                {
                    var probability = previousTask.Result;
                    _logger.LogInformation(
                        "Scenario '{Name}' processing completed with probability {Probability}.",
                        c.Scenario.Name,
                        probability);
                    await _scenarioRepository.UpdateProbabilityAsync(c.Scenario, probability);
                }

                self.Tell(new ProcessingDone());
            }, TaskContinuationOptions.ExecuteSynchronously);

            return true;
        }
        #endregion

        #region Busy
        private void Busy()
        {
            Receive<StartProcessCommand>(_ =>
            {
                Sender.Tell(new StartProcessResponse(false));
                return true;
            });
            Receive<StopProcessingCommand>(_ =>
            {
                LeaveBusyState();
                Become(Available);
                return true;
            });
            Receive<GetStatusQuery>(_ =>
            {
                Sender.Tell(new GetStatusResponse(Status.Busy));
                return true;
            });

            Receive<ProcessingDone>(_ =>
            {
                LeaveBusyState();
                Become(Available);
            });
        }

        private void LeaveBusyState()
        {
            _processingCts?.Cancel();
            _processingCts?.Dispose();
            _processingCts = null;
        }
        #endregion

        private record ProcessingDone();

        public record StartProcessCommand(Scenario Scenario);
        public record StartProcessResponse(bool Accepted);

        public record StopProcessingCommand();

        public record GetStatusQuery();
        public record GetStatusResponse(Status Status);

        public enum Status
        {
            Available,
            Busy
        }
    }
}