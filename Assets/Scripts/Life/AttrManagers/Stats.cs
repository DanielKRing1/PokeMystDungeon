using UnityEngine;

internal static class StartStats
{
    internal const float HEALTH = 100;
    internal const float ATTACK = 10;

    internal const float MAX_ATK_SPD = 1;
    internal const float MIN_ATK_SPD = 0.5f;
    internal const float DEFENSE = 5;

    internal const float VISION = 10;
    internal const float SPEED = 2;

    internal const float MAX_LUCK = 0.8f;
    internal const float MIN_LUCK = 0.2f;
}

internal static class StatsLvlUp
{
    internal const float MIN_MULTIPLIER = 0.05f;
    internal const float MAX_MULTIPLIER = 0.1f;
}

public class Stats
{
    public static Stats GenBaseStats()
    {
        float Health = Random.Range(StartStats.HEALTH / 2, StartStats.HEALTH);
        float Attack = Random.Range(StartStats.ATTACK / 2, StartStats.ATTACK);
        float AtkSpd = Random.Range(StartStats.MIN_ATK_SPD, StartStats.MAX_ATK_SPD);
        float Defense = Random.Range(StartStats.DEFENSE / 2, StartStats.DEFENSE);

        float Vision = Random.Range(StartStats.VISION / 2, StartStats.VISION);
        float Speed = Random.Range(StartStats.SPEED / 2, StartStats.SPEED);

        float Luck = Random.Range(StartStats.MIN_LUCK, StartStats.MAX_LUCK);

        return new Stats(Health, Attack, AtkSpd, Defense, Vision, Speed, Luck);
    }

    public static Stats FillBaseStats(Stats refStats = null)
    {
        Stats fillStats = GenBaseStats();

        float Health =
            (refStats != null && refStats.Health > 0) ? refStats.Health : fillStats.Health;
        float Attack =
            (refStats != null && refStats.Attack > 0) ? refStats.Attack : fillStats.Attack;
        float AtkSpd =
            (refStats != null && refStats.AtkSpd > 0) ? refStats.AtkSpd : fillStats.AtkSpd;
        float Defense =
            (refStats != null && refStats.Defense > 0) ? refStats.Defense : fillStats.Defense;

        float Vision =
            (refStats != null && refStats.Vision > 0) ? refStats.Vision : fillStats.Vision;
        float Speed = (refStats != null && refStats.Speed > 0) ? refStats.Speed : fillStats.Speed;

        float Luck = (refStats != null && refStats.Luck > 0) ? refStats.Luck : fillStats.Luck;

        return new Stats(Health, Attack, AtkSpd, Defense, Vision, Speed, Luck);
    }

    private float _health;
    public float Health
    {
        get { return _health; }
        private set { _health = value; }
    }
    private float _attack;
    public float Attack
    {
        get { return _attack; }
        private set { _attack = value; }
    }
    private float _atkspd;
    public float AtkSpd
    {
        get { return _atkspd; }
        private set { _atkspd = value; }
    }
    private float _defense;
    public float Defense
    {
        get { return _defense; }
        private set { _defense = value; }
    }

    private float _vision;
    public float Vision
    {
        get { return _vision; }
        private set { _vision = value; }
    }
    private float _speed;
    public float Speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }

    private float _luck;
    public float Luck
    {
        get { return _luck; }
        private set { _luck = value; }
    }

    public Stats(
        float Health,
        float Attack,
        float AtkSpd,
        float Defense,
        float Vision,
        float Speed,
        float Luck
    )
    {
        this.Health = Health;
        this.Attack = Attack;
        this.AtkSpd = AtkSpd;
        this.Defense = Defense;

        this.Vision = Vision;
        this.Speed = Speed;

        this.Luck = Luck;
    }

    /**
    Apply a level up multiplier to a single stat and return the new value
    */
    private float LvlUpStat(float stat)
    {
        // 1. Get stat multiplier 'floor'
        // Base luck determines luck ceiling, so
        // Higher base luck = higher potential lvl up luck
        float lvlUpLuck = Random.Range(StartStats.MIN_LUCK, this.Luck);
        // Higher lvlUpLuck = higher multiplier floor
        float multiplierFloor =
            StatsLvlUp.MIN_MULTIPLIER
            + (StatsLvlUp.MAX_MULTIPLIER - StatsLvlUp.MIN_MULTIPLIER) * lvlUpLuck;

        // 2. Get stat multiplier
        float lvlUpMultiplier = Random.Range(multiplierFloor, StatsLvlUp.MAX_MULTIPLIER);

        // 3. Increase stat
        float lvlUpStat = stat + stat * lvlUpMultiplier;

        return lvlUpStat;
    }

    /**
    Apply a multiplier to all stat values affected by leveling up
    */
    public void LevelUpStats()
    {
        this.Health = this.LvlUpStat(this.Health);
        this.Attack = this.LvlUpStat(this.Attack);
        this.AtkSpd = this.LvlUpStat(this.AtkSpd);
        this.Defense = this.LvlUpStat(this.Defense);

        this.Vision = this.LvlUpStat(this.Vision);
        this.Speed = this.LvlUpStat(this.Speed);

        // // Small chance to increase Leadership level
        // if (this.Leadership < StartStats.MAX_LEADERSHIP && Random.Range(0, 1) <= (this.Luck / 20))
        // {
        //     this.Leadership++;
        // }
    }
}
