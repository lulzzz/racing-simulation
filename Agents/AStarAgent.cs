﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Racing.Agents.Algorithms.Planning;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;

namespace Racing.Agents
{
    public class AStarAgent : IAgent
    {
        private readonly TimeSpan perceptionPeriod;
        private readonly TimeSpan simulationStep;
        private readonly Queue<RadialGoal> pointsToGo;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly BoundingSphereCollisionDetector collisionDetector;

        private Queue<IAction> plan = null;
        private IState previousPercievedState = null;

        public AStarAgent(
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track,
            TimeSpan perceptionPeriod)
        {
            this.vehicleModel = vehicleModel;
            this.motionModel = motionModel;
            this.track = track;
            this.perceptionPeriod = perceptionPeriod;
            this.simulationStep = perceptionPeriod / 20;

            collisionDetector = new BoundingSphereCollisionDetector(track, vehicleModel);

            var wayPointGoals = track.Circuit.WayPoints.Select(wayPoint => new RadialGoal(wayPoint, track.Circuit.Radius));
            pointsToGo = new Queue<RadialGoal>(wayPointGoals);
            pointsToGo.Enqueue(new RadialGoal(track.Circuit.Start, track.Circuit.Radius));
        }

        public IAction ReactTo(IState state)
        {
            if (previousPercievedState != null && pointsToGo.Count > 0)
            {
                var traveledDistance = state.Position - previousPercievedState.Position;
                var steps = (int)Math.Floor(perceptionPeriod / simulationStep);
                for (int i = 0; i < steps; i++)
                {
                    var pointBetween = previousPercievedState.Position + (double)i / steps * traveledDistance;
                    if (pointsToGo.Peek().ReachedGoal(pointBetween))
                    {
                        Console.WriteLine($"Reached waypoint {pointsToGo.Peek().Position}");
                        pointsToGo.Dequeue();

                        plan = createNewPlan(state);
                        break;
                    }
                }
            }

            previousPercievedState = state;

            if (pointsToGo.Count == 0)
            {
                return SteeringAction.Brake;
            }

            if (plan == null || plan.Count == 0)
            {
                plan = createNewPlan(state);
                if (plan == null || plan.Count == 0)
                {
                    return SteeringAction.Brake;
                }
            }

            var nextAction = plan?.Dequeue();

            if (nextAction != null && couldLeadToCrash(state, nextAction))
            {
                plan = createNewPlan(state);
                if (plan?.Count != 0)
                {
                    nextAction = plan?.Dequeue();
                }
            }

            // todo: is too off the planned trajectory?

            return nextAction ?? SteeringAction.Brake;
        }

        private IState intention(IAction action, IState state)
            => motionModel.CalculateNextState(state, action, perceptionPeriod);

        private bool couldLeadToCrash(IState state, IAction action)
        {
            for (int i = 0; i < 3; i++)
            {
                if (collisionDetector.IsCollision(state))
                {
                    return true;
                }

                state = motionModel.CalculateNextState(state, action, perceptionPeriod);
            }

            return false;
        }

        private Queue<IAction> createNewPlan(IState state)
        {
            Console.WriteLine("Replan");

            var nextWaypoint = pointsToGo.Count > 1 ? pointsToGo.Skip(1).First() : pointsToGo.Peek();

            Console.WriteLine($"Aim for {nextWaypoint.Position}");

            var planningProblem = new PlanningProblem(
                state, vehicleModel, motionModel, SteeringAction.PossibleActions, track, nextWaypoint);

            var planner = new AStarPlanner(collisionDetector, perceptionPeriod, simulationStep);
            var newPlan = planner.FindOptimalPlanFor(planningProblem);

            if (newPlan == null)
            {
                return null;
            }

            return new Queue<IAction>(newPlan);
        }
    }
}