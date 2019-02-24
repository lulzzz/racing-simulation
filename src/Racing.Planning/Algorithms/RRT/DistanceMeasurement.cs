﻿using Racing.Mathematics;
using Racing.Model;
using static System.Math;

namespace Racing.Planning.Algorithms.RRT
{
    internal sealed class DistanceMeasurement
    {
        private Length maximumDistance;

        public DistanceMeasurement(Length width, Length height)
        {
            var w = width.Meters;
            var h = height.Meters;
            maximumDistance = Sqrt(w * w + h * h);
        }

        public double DistanceBetween(IState a, IState b)
        {
            var euklideanDistance = DistanceBetween(a.Position, b.Position);
            var angleDifference = DistanceBetween(a.HeadingAngle, b.HeadingAngle);
            return euklideanDistance * euklideanDistance + angleDifference * angleDifference;
        }

        public double DistanceBetween(Vector a, Vector b)
            => (Length.Between(a, b) / maximumDistance).Meters;

        public double DistanceBetween(Angle a, Angle b)
        {
            (a, b) = a.Radians < b.Radians ? (a, b) : (b, a);
            return Min(b.Radians - a.Radians, a.Radians + (2 * PI) - b.Radians) / (2 * PI);
        }
    }
}