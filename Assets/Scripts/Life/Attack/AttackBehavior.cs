using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AttackBehavior : AttackBehaviorDecisions
{
    public static readonly List<Type> ATTACK_BEHAVIOR_TYPES = new List<Type>()
    {
        typeof(BombAttack),
        typeof(LaserAttack),
        typeof(OrbitAttack),
        typeof(SpikeAttack)
    };
    
    private Stopwatch cooldownSW;
    private Dictionary<DamageElement, DamageElement> trackedDamageElements =
        new Dictionary<DamageElement, DamageElement>();

    public virtual void Start() {
        this.cooldownSW = new Stopwatch();
    }

    // COOLDOWNS

    private float CB_GetScaledCooldown()
    {
        return this.DecideBaseCooldown() / Mathf.Sqrt(this.GetStats().AtkSpd);
    }

    // GET NEARBY ENEMIES

    /**
    Decide how to sort nearby enemies to attack
    */
    protected virtual List<GameObject> SortEnemiesInSight(Observations obs)
    {
        List<GameObject> nearbySorted = ObservationsUtils.SortEnemiesByDistance(
            this.gameObject,
            obs
        );

        return nearbySorted;
    }

    private List<GameObject> GetEnemiesInSight(Observations obs)
    {
        return this.DecidePickEnemiesInSight(this.SortEnemiesInSight(obs));
    }

    // TRACK DAMAGE ELEMENTS

    private void ClearTrackedDamageElements()
    {
        foreach (DamageElement dmgEl in this.trackedDamageElements.Values)
        {
            dmgEl.ClearDmgCooldowns();
            this.trackedDamageElements.Remove(dmgEl);
        }
    }

    private void ClearDmgCooldowns()
    {
        foreach (DamageElement dmgEl in this.trackedDamageElements.Values)
        {
            dmgEl.ClearDmgCooldowns();
        }
    }

    // INSTANTIATE DAMAGE ELEMENT

    protected void InstantiateDamageElements(List<GameObject> toAtk)
    {
        List<TargetInfo> targets = this.DecideTargets(toAtk);

        foreach (TargetInfo target in targets)
        {
            Vector3 pos = this.DecideInstantiatePosition(target);
            Quaternion rot = this.DecideInstantiateRotation(target);
            this.InstantiateDamageElement(target, pos, rot);
        }
    }

    private GameObject InstantiateDamageElement(TargetInfo target, Vector3 pos, Quaternion rot)
    {
        // 1. Instantiate GameObject
        GameObject atkGO = Instantiate(this.CB_GetDmgElGO(), pos, rot);

        // 2. Init DamageElement
        DamageElement dmgEl = atkGO.GetComponent<DamageElement>();
        dmgEl.Init(
            target,
            this.CB_MovementController,
            this.CB_DecideHandleIfReachTarget,
            this.CB_DecideReadyToDmg,
            this.CB_DecideEnemiesInHitbox,
            this.CB_GetShouldDmg,
            this.CB_GetScaledCooldown,
            this.CB_GetDmg,
            this.CB_DecideOnDmg,
            this.CB_OnDefeatEnemy,
            this.CB_HandleIfExhausted,
            this.CB_DecideAfterExecute
        );

        // 3. Potentially track DamageElement
        if (this.DecideShouldTrackDamageEl(dmgEl))
            this.trackedDamageElements.Add(dmgEl, dmgEl);

        return atkGO;
    }

    /**
    Get Attack GameObject
    */
    private GameObject CB_GetDmgElGO()
    {
        return Resources.Load(this.DecideDmgElResourcePath()) as GameObject;
    }

    public override void Execute(Observations obs)
    {
        Debug.Log(obs);
        // 0. Check cooldown
        if (!this.cooldownSW.HasElapsedStart(this.CB_GetScaledCooldown()))
            return;

        // 1. Get enemies to attack
        List<GameObject> toAtk = this.GetEnemiesInSight(obs);

        // == 0 enemies
        if (toAtk.Count == 0)
            this.ClearDmgCooldowns();

        // > 0 enemies
        this.InstantiateDamageElements(toAtk);
    }

    protected virtual void CB_MovementController(DamageElement dmgEl, TargetInfo ti)
    {
        dmgEl.GetComponent<Rigidbody>().velocity =
            this.DecideMoveSpeed() * (ti.GetTarget() - dmgEl.transform.position);
    }

    // READY TO DMG ----

    // GET SHOULD DMG ----

    /**
    Damage Element callback
    Return false if should not attack for any reason, else true
    */
    private bool CB_GetShouldDmg(GameObject go)
    {
        return true;
    }

    // GET DMG ----

    private float CB_GetDmg()
    {
        return this.DecideDmgMultiplier() * this.GetStats().Attack;
    }

    // ON DEAD ----

    private void CB_OnDefeatEnemy(GameObject enemy)
    {
        this.CollectExp(enemy);
        this.TryRecruitDeadSub(enemy);
    }

    private void CollectExp(GameObject enemy) { }

    /**
    Calls LeadershipManager to try to recruit dead enemy
    */
    private void TryRecruitDeadSub(GameObject enemy)
    {
        LeadershipPubSub pub = this.GetComponent<LeadershipManager>().pubSub;
        LeadershipPubSub sub = enemy.GetComponent<LeadershipManager>().pubSub;

        sub.TryRecruitDeadSub(pub);
    }

    // HANDLE IF EXHAUSTED ----

    private bool CB_HandleIfExhausted(int hitCount, DamageElement dmgEl)
    {
        bool isExhausted = CheckIfExhausted(hitCount, dmgEl);
        if (isExhausted)
            HandleDestroy(dmgEl);

        return isExhausted;
    }

    protected virtual bool CheckIfExhausted(int hitCount, DamageElement dmgEl)
    {
        return hitCount >= this.DecideMaxHitCount();
    }

    // DESTROY ----

    protected void HandleDestroy(DamageElement dmgEl)
    {
        Destroy(dmgEl);
    }

    // POSITIONING

    protected Quaternion FaceTarget(TargetInfo target)
    {
        return Quaternion.Euler(target.GetTarget() - this.transform.position);
    }
}
