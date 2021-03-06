﻿using System;
using Racing.Model;

namespace Racing.Planning.Domain
{
    internal sealed class ActionTrajectory : IActionTrajectory
    {
        public ActionTrajectory(TimeSpan time, IState state, IAction? action)
        {
            Time = time;
            State = state;
            Action = action;
        }

        public TimeSpan Time { get; }
        public IState State { get; }
        public IAction? Action { get; }
    }
}
