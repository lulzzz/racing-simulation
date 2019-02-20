﻿using Racing.Mathematics;
using System;
using static System.Math;

namespace Racing.Model.Vehicle
{
    public sealed class DynamicModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;
        private readonly TimeSpan simulationTime;

        public DynamicModel(IVehicleModel vehicle)
        {
            this.vehicle = vehicle;
        }

        public IState CalculateNextState(IState state, IAction action, TimeSpan time)
        {
            while (time > TimeSpan.Zero)
            {
                var step = time < simulationTime ? time : simulationTime;
                time -= simulationTime;

                state = calculateNextState(state, action, step);
            }

            return state;
        }

        private IState calculateNextState(IState state, IAction action, TimeSpan time)
        {
            var seconds = time.TotalSeconds;

            var acceleration = action.Throttle * vehicle.Acceleration;
            var steeringAcceleration = action.Steering * vehicle.SteeringAcceleration;

            var ds = seconds * acceleration;
            var speed = Clamp(state.Speed + ds, vehicle.MinSpeed, vehicle.MaxSpeed);

            var da = seconds * steeringAcceleration.Radians;
            var steeringAngle = Clamp(state.SteeringAngle.Radians + da, vehicle.MinSteeringAngle.Radians, vehicle.MaxSteeringAngle.Radians);

            var velocity = new Point(
                x: speed * Cos(steeringAngle) * Cos(state.HeadingAngle.Radians),
                y: speed * Cos(steeringAngle) * Sin(state.HeadingAngle.Radians));

            Angle headingAngularVelocity = (speed / vehicle.Length) * Sin(steeringAngle);

            return new VehicleState(
                position: state.Position + seconds * velocity,
                heading: state.HeadingAngle + seconds * headingAngularVelocity,
                speed: speed,
                steeringAngle: steeringAngle);
        }
    }
}
