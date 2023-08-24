using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : AttrManager
{
    private int _level;
    public int Level
    {
        get { return _level; }
        private set { _level = value; }
    }

    private int _exp;
    public int Exp
    {
        get { return _exp; }
        private set { _exp = value; }
    }

    public override void Init(
        int level,
        Stats stats,
        int leadership = -1,
        PubSub<LeadershipManager> leader = null
    )
    {
        this.Level = level;
        this.Exp = 0;
    }

    public void EarnExp(int exp)
    {
        // 1. Add exp
        this.Exp += _exp;

        // 2. Level up
        while (this.Exp >= 100)
        {
            this.Level++;
            this.Exp -= 100;

            this.LevelUp();
        }
    }

    private void LevelUp()
    {
        LevelUpSensitive[] luss = this.GetComponents<LevelUpSensitive>();

        foreach (LevelUpSensitive lus in luss)
        {
            lus.LevelUp();
        }
    }
}
