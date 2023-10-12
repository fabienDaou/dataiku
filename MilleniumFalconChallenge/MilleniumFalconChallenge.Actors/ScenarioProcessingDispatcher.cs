﻿using Akka.Actor;

namespace MilleniumFalconChallenge.Actors
{
    public class ScenarioProcessingDispatcher : IScenarioProcessingDispatcher
    {
        private readonly IActorRef _processorSupervisorRef;

        public ScenarioProcessingDispatcher(IActorRef processorSupervisorRef)
        {
            _processorSupervisorRef = processorSupervisorRef ?? throw new ArgumentNullException(nameof(processorSupervisorRef));
        }

        public void Dispatch(Scenario scenario)
        {
            _processorSupervisorRef.Tell(new ScenarioProcessorsSupervisorActor.ProcessScenario(scenario));
        }
    }
}