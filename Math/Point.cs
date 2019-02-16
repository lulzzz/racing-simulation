﻿using System;
using static System.Math;

namespace Racing.Mathematics
{
    public struct Point : IEquatable<Point>
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point CalculateNormal()
        {
            var normalized = Normalize();
            return new Point(-normalized.Y, normalized.X);
        }

        public Point Normalize()
        {
            var length = Length();
            return (1 / length) * this;
        }

        public double Length()
            => Sqrt(X * X + Y * Y);

        public Point Rotate(Angle angle)
            => new Point(
                x: X * Cos(angle.Radians) - Y * Sin(angle.Radians),
                y: X * Sin(angle.Radians) + Y * Cos(angle.Radians));

        public Point Rotate(Point center, Angle angle)
            => (this - center).Rotate(angle) + center;

        public double DistanceSq(Point other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        public double Cross(Point other)
            => X * other.Y - Y * other.X;

        public double Dot(Point other)
            => X * other.X + Y * other.Y;

        public Angle Direction()
            => Sin(Y);

        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y);

        public static Point operator -(Point a, Point b)
            => new Point(a.X - b.X, a.Y - b.Y);

        public static Point operator *(double scale, Point a)
            => new Point(scale * a.X, scale * a.Y);

        public override string ToString()
            => FormattableString.Invariant($"Point[{X}, {Y}]");

        public static bool operator ==(Point a, Point b)
            => a.Equals(b);

        public static bool operator !=(Point a, Point b)
            => !(a == b);

        public override bool Equals(object other)
        {
            if (other != null && other is Point point)
            {
                return Equals(point);
            }

            return false;
        }

        public bool Equals(Point other)
            => other.X == X && other.Y == Y;

        public override int GetHashCode()
            => X.GetHashCode() + 23 * Y.GetHashCode();
    }
}