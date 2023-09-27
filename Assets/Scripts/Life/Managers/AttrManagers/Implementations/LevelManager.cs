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
        this.Exp = 0;

        this.Level = level;
        for (int i = 1; i <= level; i++)
        {
            this.LevelUp();
        }
    }

    // void Start()
    // {
    //     int level = this.Level;
    //     this.Level = 0;

    //     for (int i = 1; i <= level; i++)
    //     {
    //         this.LevelUp();
    //     }
    // }

    public void EarnExp(int exp)
    {
        // 1. Add exp
        this.Exp += exp;

        // 2. Level up
        while (this.Exp >= this.CalcNeededExp())
        {
            this.Exp -= (int)this.CalcNeededExp();

            this.LevelUp();
        }

        this.GetComponentInChildren<InfoInterface>().UpdateExpBar(Exp, this.CalcNeededExp());
    }

    private void LevelUp()
    {
        this.Level++;

        ILevelUpSensitive[] luss = this.GetComponents<ILevelUpSensitive>();
        foreach (ILevelUpSensitive lus in luss)
        {
            lus.OnLevelUp();
        }

        this.GetComponentInChildren<InfoInterface>().UpdateLevelText(this.Level);
    }

    /**
    Exp needed to level up
    */
    public int CalcNeededExp()
    {
        return (int)(Mathf.Pow(this.Level, 1.2f));
    }

    /**
    Exp earned for killing me
    */
    public int CalcEarnedExp()
    {
        return (int)(Mathf.Ceil(this.CalcNeededExp() / 5));
    }
}
