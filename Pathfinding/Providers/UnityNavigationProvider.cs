using System;
using ClownMeister.UnityEssentials.Core;
using ClownMeister.UnityEssentials.Logger;
using ClownMeister.UnityEssentials.Logger.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace ClownMeister.UnityEssentials.Pathfinding.Providers
{
    public class UnityNavigationProvider : INavigationProvider
    {
        private readonly NavMeshAgent _agent;
        private readonly ITickManager _tickManager;
        private Action _onFinishAction;

        private State _state = State.Stopped;

        public UnityNavigationProvider(NavMeshAgent agent, ITickManager tickManager)
        {
            _agent = agent;
            _tickManager = tickManager;
            _tickManager.SubscribeToTick(10, _ => TickUpdate());
        }

        public void Move(Vector3 target, Action onArrivedAtPosition = null)
        {
            DLogger.Log("NavigationProvider setting destination", LogChannel.Navigation);
            _state = State.Moving;
            _onFinishAction = onArrivedAtPosition;
            _agent.SetDestination(target);
        }

        public bool IsMoving()
        {
            return _state != State.Stopped;
        }

        public Vector3 GetPosition()
        {
            return _agent.transform.position;
        }

        public void Stop(Action onStop = null)
        {
            DLogger.Log("NavigationProvider is stopping agent", LogChannel.Navigation);
            _state = State.Stopped;
            _agent.ResetPath();
        }

        private void TickUpdate()
        {
            switch (_state)
            {
                case State.Stopped:
                    break;
                case State.Moving:
                    CheckIfPathFinished();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckIfPathFinished()
        {
            DLogger.Log("NavigationProvider checking path state", LogChannel.Navigation);

            // Check if we've reached the destination
            if (_agent.pathPending) return;
            if (!(_agent.remainingDistance <= _agent.stoppingDistance)) return;
            if (_agent.hasPath && _agent.velocity.sqrMagnitude != 0f) return;
            DLogger.Log("NavigationProvider: Agent finished path", LogChannel.Navigation);
            FinishPath();
        }

        private void FinishPath()
        {
            DLogger.Log("NavigationProvider finishing path", LogChannel.Navigation);
            Stop();
            _onFinishAction?.Invoke();
        }

        private enum State
        {
            Stopped,
            Moving
        }
    }
}