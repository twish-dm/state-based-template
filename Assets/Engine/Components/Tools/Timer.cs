
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Engine.Components.Tools
{
    public class Timer
    {
        static private Dictionary<string, CancellationTokenSource> m_TimerMap;
        static Timer()
        {
            m_TimerMap = new Dictionary<string, CancellationTokenSource>();
        }
        /// <summary>
        /// Таймер выполняеться один раз, основанный на async
        /// </summary>
        /// <param name="name">имя, нужно для остановки</param>
        /// <param name="timer"></param>
        /// <param name="onComplete"></param>
        async static public void Once(string name, float timer, UnityAction onComplete, bool invokeIfCancelled = false)
        {
            if (m_TimerMap.ContainsKey(name))
                Stop(name);

            m_TimerMap.Add(name, new CancellationTokenSource());
            try
            {
                await Task.Delay(Mathf.RoundToInt(timer * 1000), m_TimerMap[name].Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                if (invokeIfCancelled)
                    onComplete?.Invoke();
            }
            finally
            {
                if (m_TimerMap.ContainsKey(name))
                {
                    m_TimerMap[name]?.Cancel();
                    m_TimerMap[name] = null;
                    m_TimerMap.Remove(name);
                }
            }
        }
        /// <summary>
        /// Таймер выполняеться несколько раз или до остановки, основанный на async
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timer"></param>
        /// <param name="repeat"></param>
        /// <param name="onComplete"></param>
        async static public void Loop(string name, float timer, UnityAction onLoopComplete, int repeat = -1, UnityAction onComplete = null, bool invokeIfCancelled = false)
        {
            if (m_TimerMap.ContainsKey(name))
                Stop(name);
            m_TimerMap.Add(name, new CancellationTokenSource());
            try
            {
                await Timer();
            }
            catch (OperationCanceledException)
            {
                if (invokeIfCancelled)
                    try
                    {
                        onComplete?.Invoke();
                    }
                    catch (Exception)
                    {
                        Debug.LogError("Invoked after destroyed");
                    }
            }
            finally
            {
                if (m_TimerMap.ContainsKey(name))
                {
                    m_TimerMap[name].Cancel();
                    m_TimerMap[name] = null;
                    m_TimerMap.Remove(name);
                }
            }
            try
            {
                onComplete?.Invoke();
            }
            catch (Exception)
            {
                Debug.LogError("Invoked after destroyed");
            }
            async Task Timer()
            {
                int iteration = repeat < 0 ? -1 : 0;
                repeat = repeat < 0 ? 0 : repeat;
                while (iteration < repeat)
                {
                    await Task.Delay(Mathf.RoundToInt(timer * 1000), m_TimerMap[name].Token);
                    try
                    {
                        onLoopComplete?.Invoke();
                    }
                    catch (Exception)
                    {
                        Debug.LogError("Invoked after destroyed");
                    }
                }
            }
        }
        static public void Stop(string name)
        {
            if (m_TimerMap.TryGetValue(name, out CancellationTokenSource value))
            {
                value.Cancel();
                value = null;
                m_TimerMap[name] = null;
                m_TimerMap.Remove(name);
            }
        }
        static public void Stop()
        {
            foreach (string key in m_TimerMap.Keys)
            {
                Stop(key);
            }
        }
    }
}