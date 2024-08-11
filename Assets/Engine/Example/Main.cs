namespace StateEngine.Example
{
    using StateEngine;

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Main : Engine
    {
        // Start is called before the first frame update
        void Start()
        {
            stater.Add(new MainState());
            stater.Start("MainState");
        }
    }
}