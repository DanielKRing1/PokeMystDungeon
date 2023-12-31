using System;
using System.Collections.Generic;
using UnityEngine;

/**
Use this class to define some target

Provide a GameObject for a live position, or
Provide a Vector3 for an unchanging position
*/
public class TargetInfo
{
    private GameObject targetGO;
    private Vector3 targetPos;

    public TargetInfo(GameObject go)
    {
        this.targetGO = go;
    }

    public TargetInfo(Vector3 pos)
    {
        this.targetPos = pos;
    }

    public Vector3 GetTarget()
    {
        if (this.targetGO != null)
            return this.targetGO.transform.position;
        if (this.targetPos != null)
            return this.targetPos;

        throw new Exception("TargetInfo has neither GameObject nor Position");
    }
}

public class DamageElement : MonoBehaviour
{
    private bool isInitted = false;

    // INTERNAL STATE

    private int hitCount = 0;

    // Maps GameObject to the last time it was damaged by this DamageElement
    private Dictionary<GameObject, float> dmgCooldowns = new Dictionary<GameObject, float>();

    // ATTACK BEHAVIOR PROPS

    // Movement controls
    private TargetInfo targetInfo;
    private Action<DamageElement, TargetInfo> movementController;
    private Func<DamageElement, TargetInfo, bool> handleIfReachTarget;

    // Timing controls
    private Func<DamageElement, TargetInfo, bool> readyToDmg;
    private Func<GameObject, bool> getShouldDmg;
    private Func<float> getCooldown;

    // Damage controls
    public delegate bool GetEnemiesInHitboxFunc(
        DamageElement dmgEl,
        TargetInfo target,
        out List<GameObject> list
    );
    private GetEnemiesInHitboxFunc getEnemiesInHitbox;
    private Func<float> getDmg;

    // Event callbacks
    private Action<int, GameObject> onDmg;
    private Action<GameObject> onDead;
    private Func<int, DamageElement, bool> handleIfExhausted;
    private Func<DamageElement, bool> afterExecute;
    private Action<DamageElement> beforeDestroyGameObject;
    private GameObject parent;

    public void Init(
        TargetInfo targetInfo,
        Action<DamageElement, TargetInfo> movementController,
        Func<DamageElement, TargetInfo, bool> handleIfReachTarget,
        Func<DamageElement, TargetInfo, bool> readyToDmg,
        GetEnemiesInHitboxFunc getEnemiesInHitbox,
        Func<GameObject, bool> getShouldDmg,
        Func<float> getCooldown,
        Func<float> getDmg,
        Action<int, GameObject> onDmg,
        Action<GameObject> onDead,
        Func<int, DamageElement, bool> handleIfExhausted,
        Func<DamageElement, bool> afterExecute,
        Action<DamageElement> beforeDestroyGameObject,
        GameObject parent
    )
    {
        this.targetInfo = targetInfo;
        this.movementController = movementController;
        this.handleIfReachTarget = handleIfReachTarget;

        this.readyToDmg = readyToDmg;
        this.getEnemiesInHitbox = getEnemiesInHitbox;

        this.getShouldDmg = getShouldDmg;
        this.getCooldown = getCooldown;

        this.getDmg = getDmg;

        this.onDmg = onDmg;
        this.onDead = onDead;
        this.handleIfExhausted = handleIfExhausted;
        this.afterExecute = afterExecute;
        this.beforeDestroyGameObject = beforeDestroyGameObject;

        this.parent = parent;

        // Init
        this.isInitted = true;
    }

    public void Update()
    {
        if (!this.isInitted)
            return;

        this.ExecuteAttack();
    }

    private void ExecuteAttack()
    {
        this.ExecuteMovementControls();

        if (!this.ReadyToDmg())
            return;

        // 3. Damage enemies within hitbox
        List<GameObject> enemiesInHitbox;
        // None in range
        // TODO Sept. 16, 2023: Referencing AttackBehavior callbacks from DamageElement... AttackBehaviro may have already been destroyed
        // In DestroyManager, disable GameObject until all async OnDestroy methods return
        // (Call AttackBehavior to return once DamageElements are destroyed... how to track this from AttackBehavior?)
        if (!this.getEnemiesInHitbox(this, this.targetInfo, out enemiesInHitbox))
            return;

        this.ExecuteDamageControls(enemiesInHitbox);

        // 4. Do something after damaging all nearby enemies
        // (Explode or smthn)
        this.ExecuteCleanup();
    }

    private void ExecuteMovementControls()
    {
        // 1. Move
        this.movementController(this, this.targetInfo);
        this.handleIfReachTarget(this, this.targetInfo);
    }

    private bool ReadyToDmg()
    {
        // (not at target position, for aoe/bomb attacks)
        return this.readyToDmg(this, this.targetInfo);
    }

    private bool ShouldDmg(GameObject enemy)
    {
        // Should not damage (eg is an ally)
        if (!this.getShouldDmg(enemy))
            return false;

        // Damaged recently
        if (this.dmgCooldowns.ContainsKey(enemy))
        {
            // Cooldown expired
            if (Time.time - this.dmgCooldowns[enemy] >= this.getCooldown())
            {
                this.dmgCooldowns[enemy] = Time.time;
                return true;
            }
            // Cooldown not expired
            else
                return false;
        }
        // Not damaged recently -> no cooldown -> create cooldown
        else
        {
            this.dmgCooldowns.Add(enemy, Time.time);
            return true;
        }
    }

    private void ExecuteDamageControls(List<GameObject> enemiesInHitbox)
    {
        foreach (GameObject enemy in enemiesInHitbox)
        {
            if (!this.ShouldDmg(enemy))
                continue;

            // 1. Get enemy health component
            HealthFunction hf = enemy.GetComponent<HealthFunction>();

            // 2. Apply damage to enemy
            LevelManager parentLevelM = this.parent.GetComponent<LevelManager>();
            LeadershipManager parentLeaderM = this.parent.GetComponent<LeadershipManager>();

            Action<int> earnExpCB = (int exp) =>
            {
                parentLevelM.EarnExp(exp);
            };
            Func<LeadershipPubSub, bool> tryRecruitDeadSubCB = (LeadershipPubSub sub) =>
            {
                LeadershipPubSub pub = parentLeaderM.pubSub;
                bool recruitSuccess = sub.TryRecruitOnceDead(pub);
                return recruitSuccess;
            };

            // A damage number value would not be enough
            // The getDmg Function is important bcus it references the Caller's Stats, so
            //      it will produce 'live' damage, should the Caller's Stats change
            bool dead = hf.Hurt((int)this.getDmg(), earnExpCB, tryRecruitDeadSubCB);

            // 3. Do something after damaging enemy
            this.onDmg(this.hitCount, enemy);

            // 4. Do something on enemy death (collect exp and try to recruit to team)
            if (dead)
                this.onDead(enemy);

            // 5. Handle running out of "ammo"
            this.hitCount++;
            if (this.handleIfExhausted(this.hitCount, this))
                break;
        }
    }

    private void ExecuteCleanup()
    {
        bool shouldDestroy = this.afterExecute(this);

        if (shouldDestroy)
            this.DestroyGameObject();
    }

    public void DestroyGameObject()
    {
        if (this == null)
            return;

        this.beforeDestroyGameObject(this);
        Destroy(this.gameObject);
    }

    public void ClearDmgCooldowns()
    {
        if (this.dmgCooldowns.Count > 0)
            this.dmgCooldowns = new Dictionary<GameObject, float>();
    }
}
