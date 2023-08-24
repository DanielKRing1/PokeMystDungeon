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
        return this.targetGO != null ? this.targetGO.transform.position : this.targetPos;
    }
}

public class DamageElement : MonoBehaviour
{
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
    public delegate bool GetEnemiesInHitboxFunc(TargetInfo target, out List<GameObject> list);
    private GetEnemiesInHitboxFunc getEnemiesInHitbox;
    private Func<float> getDmg;

    // Event callbacks
    private Action<int, GameObject> onDmg;
    private Action<GameObject> onDead;
    private Func<int, DamageElement, bool> handleIfExhausted;
    private Action<DamageElement> afterExecute;

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
        Action<DamageElement> afterExecute
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
    }

    public void Update() {
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
        if (!this.getEnemiesInHitbox(this.targetInfo, out enemiesInHitbox))
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
            // A damage number value would not be enough
            // The getDmg Function is important bcus it references the Caller's Stats, so
            //      it will produce 'live' damage, should the Caller's Stats change
            bool dead = hf.Hurt((int)this.getDmg());

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
        this.afterExecute(this);
    }

    public void ClearDmgCooldowns()
    {
        if (this.dmgCooldowns.Count > 0)
            this.dmgCooldowns = new Dictionary<GameObject, float>();
    }
}
