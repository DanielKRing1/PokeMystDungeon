using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch
{
    private float StartTime;

    public Stopwatch()
    {
        StartTime = Time.time;
    }

    public float Start()
    {
        this.StartTime = Time.time;

        return this.StartTime;
    }

    public float GetElapsed()
    {
        return Time.time - this.StartTime;
    }

    public bool HasElapsed(float dt)
    {
        return this.GetElapsed() >= dt;
    }

    public bool HasElapsedStart(float dt)
    {
        bool hasElapsed = this.HasElapsed(dt);

        if (hasElapsed)
            this.Start();

        return hasElapsed;
    }
}
