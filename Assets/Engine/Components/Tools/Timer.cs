using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace StateEngine.Utilities
{
    /// <summary>
    /// Named timers driven by the Unity main thread. Starting a timer with an existing id replaces it.
    /// Time is measured in unscaled real time.
    /// </summary>
    public static class Timer
    {
        private static readonly Dictionary<string, CancellationTokenSource> s_Timers =
            new Dictionary<string, CancellationTokenSource>();

        public static bool IsRunning(string id) => s_Timers.ContainsKey(id);

        public static void Once(string id, float delay, Action onComplete, bool invokeIfStopped = false)
        {
            Run(id, delay, 1, null, null, onComplete, invokeIfStopped);
        }

        /// <param name="repeatCount">Number of iterations, or a negative value to repeat until stopped.</param>
        /// <param name="onTick">Receives normalized progress of the current iteration every frame.</param>
        public static void Loop(
            string id,
            float interval,
            Action onIteration,
            int repeatCount = -1,
            Action onComplete = null,
            Action<float> onTick = null,
            bool invokeIfStopped = false)
        {
            Run(id, interval, repeatCount, onIteration, onTick, onComplete, invokeIfStopped);
        }

        public static void Stop(string id)
        {
            if (s_Timers.TryGetValue(id, out CancellationTokenSource source))
            {
                s_Timers.Remove(id);
                source.Cancel();
            }
        }

        public static void StopAll()
        {
            var sources = new List<CancellationTokenSource>(s_Timers.Values);
            s_Timers.Clear();

            foreach (CancellationTokenSource source in sources)
                source.Cancel();
        }

        private static void Run(
            string id,
            float interval,
            int repeatCount,
            Action onIteration,
            Action<float> onTick,
            Action onComplete,
            bool invokeIfStopped)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Timer id cannot be empty.", nameof(id));

            if (interval < 0f)
                throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval cannot be negative.");

            Stop(id);

            var source = new CancellationTokenSource();
            s_Timers[id] = source;

            Execute(id, source, interval, repeatCount, onIteration, onTick, onComplete, invokeIfStopped);
        }

        private static async void Execute(
            string id,
            CancellationTokenSource source,
            float interval,
            int repeatCount,
            Action onIteration,
            Action<float> onTick,
            Action onComplete,
            bool invokeIfStopped)
        {
            try
            {
                bool finished = await RunIterations(interval, repeatCount, onIteration, onTick, source.Token);

                if (finished || invokeIfStopped)
                    onComplete?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (s_Timers.TryGetValue(id, out CancellationTokenSource current) && current == source)
                    s_Timers.Remove(id);

                source.Dispose();
            }
        }

        private static async Task<bool> RunIterations(
            float interval,
            int repeatCount,
            Action onIteration,
            Action<float> onTick,
            CancellationToken token)
        {
            for (int i = 0; repeatCount < 0 || i < repeatCount; i++)
            {
                float startTime = Time.realtimeSinceStartup;
                float elapsed;

                do
                {
                    await Task.Yield();

                    if (token.IsCancellationRequested || !Application.isPlaying)
                        return false;

                    elapsed = Time.realtimeSinceStartup - startTime;
                    onTick?.Invoke(interval > 0f ? Mathf.Clamp01(elapsed / interval) : 1f);
                }
                while (elapsed < interval);

                onIteration?.Invoke();
            }

            return true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlayModeEnter() => StopAll();
    }
}
