﻿namespace Racing.Model.CollisionDetection
{
    public interface ICollisionDetector
    {
        bool IsCollision(IState state);
    }
}
