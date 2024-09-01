using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Core
{
    public interface ITickManager
    {
        void SetGameSpeed(float scale = 1f);
        void PauseGame();
        void UnpauseGame();
        void TogglePause();

        event EventHandler<TickManager.OnTickEventArgs> OnTick;
        void SubscribeToTick(int tickInterval, Action<int> callback);
    }

    public class TickManager : MonoBehaviour, ITickManager
    {
        private const int TargetTps = 20;

        private readonly Dictionary<int, List<Action<int>>> _tickSubscribers = new();

        private bool _isPaused;

        private int _tick;
        private float _tickTimer;
        private float _tickTimerMax;

        private void Awake()
        {
            this._tickTimerMax = 1f / TargetTps;
        }

        private void Update()
        {
            this._tickTimer += Time.deltaTime;
            if (this._tickTimer < this._tickTimerMax) return;

            this._tickTimer -= this._tickTimerMax;
            this._tick++;
            OnTickEventArgs args = new() { Tick = this._tick };
            OnTick?.Invoke(this, args);

            foreach (KeyValuePair<int, List<Action<int>>> keyValuePair in this._tickSubscribers)
                if (this._tick % keyValuePair.Key == 0)
                    foreach (Action<int> callback in keyValuePair.Value)
                        callback.Invoke(this._tick);
        }

        public event EventHandler<OnTickEventArgs> OnTick;

        public void SubscribeToTick(int tickInterval, Action<int> callback)
        {
            if (!this._tickSubscribers.ContainsKey(tickInterval)) this._tickSubscribers[tickInterval] = new List<Action<int>>();
            this._tickSubscribers[tickInterval].Add(callback);
        }

        public void PauseGame()
        {
            if (this._isPaused) return;
            Time.timeScale = 0;
            this._isPaused = true;
        }

        public void UnpauseGame()
        {
            if (!this._isPaused) return;
            Time.timeScale = 1; // Restore to its previous value, assuming normal speed is 1
            this._isPaused = false;
        }

        public void TogglePause()
        {
            if (this._isPaused)
                UnpauseGame();
            else
                PauseGame();
        }

        public void SetGameSpeed(float scale = 1f)
        {
            if (scale == 0)
            {
                PauseGame();
            }
            else
            {
                Time.timeScale = scale;
                if (this._isPaused) this._isPaused = false;
            }
        }

        public class OnTickEventArgs : EventArgs
        {
            public int Tick;
        }
    }
}