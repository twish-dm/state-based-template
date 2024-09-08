namespace StateEngine.Example
{
    using Assets.Engine.Components;

    using StateEngine;

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class Main : Engine
    {
        // Start is called before the first frame update
        void Start()
        {
            stater.Add(new MainState());
            stater.Start("MainState");


            Timer.Once("test", 1f, () => { Debug.Log("test"); });
            Timer.Loop("loop", 1f, () => { Debug.Log("loop"); });

            Timer.Once("test1", 5f, () => { Timer.Stop("loop");  Debug.Log("stop"); });
        }
    }
}