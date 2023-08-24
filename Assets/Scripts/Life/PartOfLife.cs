using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartOfLife : MonoBehaviour
{
    protected Stats GetStats()
    {
        return this.GetComponent<StatsManager>().GetStats();
    }

    public abstract void Execute(Observations obs);
}
