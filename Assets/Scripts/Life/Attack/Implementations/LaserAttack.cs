using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : AttackBehavior
{
    private const float COOLDOWN = 2f;
    private const float CHARGE_TIME = 1f;
    private Stopwatch readySW;

    private bool scheduledForDestroy = false;

    public override void Start()
    {
        base.Start();
        this.readySW = new Stopwatch();
    }

    /**
    Return false so Base Class does not Destroy DamageElement, instead
    Destroy DamageElement after a delay (using Coroutine)
    */
    protected override bool CB_DecideAfterExecute(DamageElement dmgEl)
    {
        if (this.scheduledForDestroy)
            return false;

        IEnumerator ScheduleDestroy(DamageElement dmgEl)
        {
            yield return new WaitForSeconds(2.0f); // Wait for 2 seconds
            dmgEl.DestroyGameObject();
        }
        StartCoroutine(ScheduleDestroy(dmgEl));
        this.scheduledForDestroy = true;

        return false;
    }

    protected override string DecideDmgElResourcePath()
    {
        return "Prefabs/DmgEl";
    }

    protected override float DecideBaseCooldown()
    {
        return LaserAttack.COOLDOWN;
    }

    protected override float DecideDmgMultiplier()
    {
        return 1.25f;
    }

    protected override bool CB_DecideEnemiesInHitbox(TargetInfo target, out List<GameObject> list)
    {
        Vector3 direction = target.GetTarget() - this.transform.position;
        return HitboxUtils.GetEnemiesInSphereCastHitbox(
            out list,
            this.transform.position,
            this.DecideHitboxRadius(),
            direction,
            // TODO Aug 26, 2023: Define this somewhere
            10f,
            this.GetComponent<LeadershipManager>().GetRootLeader()
        );
    }

    protected override bool CB_DecideHandleIfReachTarget(DamageElement dmgEl, TargetInfo target)
    {
        // Do nothing
        return true;
    }

    protected override float DecideHitboxRadius()
    {
        return 1f;
    }

    protected override float DecideMaxHitCount()
    {
        return Mathf.Infinity;
    }

    protected override float DecideMoveSpeed()
    {
        return 0f;
    }

    protected override void CB_DecideOnDmg(int hitCount, GameObject enemy)
    {
        // Do nothing
    }

    /**
    List with a single element, so a single laser is spawned
    
    Get first enemy, but
    Keep in mind that enemies do not matter for the Player, as this is an "aimed" attack
    */
    protected override List<GameObject> DecidePickEnemiesInSight(List<GameObject> sortedEnemies)
    {
        return sortedEnemies.Count == 0
            ? sortedEnemies
            : new List<GameObject>() { sortedEnemies[0] };
    }

    /**
    Return the current position of the nearest enemy -> The laser shoots at where the enemy "was"; it does not follow the enemy
    
    (For Player, return the current position of the cursor)
    */
    protected override List<TargetInfo> DecideTargets(List<GameObject> toAtk)
    {
        // Player
        if (this.gameObject.tag == "Player")
            return new List<TargetInfo>()
            {
                new TargetInfo(Camera.main.ScreenToWorldPoint(Input.mousePosition))
            };

        // Not Player
        return toAtk.Count == 0
            ? new List<TargetInfo>()
            : new List<TargetInfo>() { new TargetInfo(toAtk[0].transform.position) };
    }

    protected override Vector3 DecideInstantiatePosition(TargetInfo target)
    {
        return this.transform.position;
    }

    protected override Quaternion DecideInstantiateRotation(TargetInfo target)
    {
        Vector3 dir = target.GetTarget() - this.DecideInstantiatePosition(target);
        return Quaternion.Euler(dir);
    }

    protected override bool CB_DecideReadyToDmg(DamageElement dmgEl, TargetInfo ti)
    {
        return this.readySW.HasElapsed(LaserAttack.CHARGE_TIME);
    }

    protected override bool DecideShouldTrackDamageEl(DamageElement dmgEl)
    {
        return false;
    }
}
