using System;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Pathfinding.Providers
{
    public interface INavigationProvider
    {
        void Move(Vector3 target, Action onArrivedAtPosition = null);
        bool IsMoving();
        Vector3 GetPosition();
        void Stop(Action onStop = null);
    }
}