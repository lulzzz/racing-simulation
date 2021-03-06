﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Racing.Model;
using Racing.Model.Simulation;
using System;

namespace Racing.IO.Model
{
    internal static class SerializableEventFactory
    {
        public static ISerializableEvent From(IEvent logEvent)
        {
            switch (logEvent)
            {
                case IActionSelectedEvent actionSelected:
                    return new ActionSelectedEvent(actionSelected);

                case IStateUpdatedEvent stateUpdated:
                    return new StateUpdatedEvent(stateUpdated);

                case ISimulationEndedEvent ended:
                    return new SimulationEndedEvent(ended);

                default:
                    throw new NotSupportedException($"{logEvent.GetType().FullName} is not supported and can't be serialized.");
            }
        }

        private sealed class ActionSelectedEvent : ISerializableEvent
        {
            private readonly IActionSelectedEvent original;

            public ActionSelectedEvent(IActionSelectedEvent original)
            {
                this.original = original;
            }

            public string Type => "actionSelected";
            public double Time => original.Time.TotalSeconds;
            public IAction Action => original.Action;
        }

        private sealed class StateUpdatedEvent : ISerializableEvent
        {
            private readonly IStateUpdatedEvent original;

            public StateUpdatedEvent(IStateUpdatedEvent original)
            {
                this.original = original;
            }

            public string Type => "stateUpdated";
            public double Time => original.Time.TotalSeconds;
            public IState State => original.State;
        }

        private sealed class SimulationEndedEvent : ISerializableEvent
        {
            private readonly ISimulationEndedEvent original;

            public SimulationEndedEvent(ISimulationEndedEvent original)
            {
                this.original = original;
            }

            public string Type => "simulationEnded";
            public double Time => original.Time.TotalSeconds;

            [JsonConverter(typeof(StringEnumConverter))]
            public Result Result => original.Result;
        }
    }
}
