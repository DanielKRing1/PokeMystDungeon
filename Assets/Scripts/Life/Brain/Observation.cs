using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observations
{
    // nearbyEnemies [[leadership lvl1], [leadership lvl2], [leadership lvl3], …]
    public List<List<GameObject>> nearbyEnemies = new List<List<GameObject>>();
    public List<GameObject> nearbyAllies = new List<GameObject>();
}
