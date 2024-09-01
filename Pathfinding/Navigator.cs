using System;
using ClownMeister.UnityEssentials.Logger;
using ClownMeister.UnityEssentials.Logger.Enums;
using ClownMeister.UnityEssentials.Pathfinding.Providers;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Pathfinding
{
    public class Navigator
    {
        private readonly INavigationProvider _navigationProvider;

        public Navigator(INavigationProvider navigationProvider)
        {
            this._navigationProvider = navigationProvider;
        }

        public void Move(Vector3 target, Action onArrivedAtPosition = null)
        {
            DLogger.Log("Navigator issuing move command", LogChannel.Navigation);
            this._navigationProvider.Move(target, onArrivedAtPosition);
        }

        public void Stop(Action onStop = null)
        {
            this._navigationProvider.Stop(onStop);
        }

        public bool IsMoving()
        {
            return this._navigationProvider.IsMoving();
        }

        public Vector3 GetPosition()
        {
            return this._navigationProvider.GetPosition();
        }
    }
}