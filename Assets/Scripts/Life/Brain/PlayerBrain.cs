using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : Brain {
    protected override void Start() {
        Debug.Log("Start-----------------");
        int level = 1;
        Stats baseStats = Stats.GenBaseStats();
        int leadership = LeadershipManager.MAX_LEADERSHIP;
        PubSub<LeadershipManager> leader = null;

        this.Init(level, baseStats, leadership, leader);
    }
}
