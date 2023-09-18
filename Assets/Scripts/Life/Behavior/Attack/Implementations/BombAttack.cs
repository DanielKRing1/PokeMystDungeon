using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAttack : AttackBehavior
{
    protected override bool CB_DecideAfterExecute(DamageElement dmgEl)
    {
        // Do nothing
        return true;
    }

    protected override string DecideDmgElResourcePath()
    {
        return "Prefabs/DmgEl";
    }

    protected override float DecideBaseCooldown()
    {
        return 3f;
    }

    protected override float DecideDmgMultiplier()
    {
        return 2f;
    }

    protected override bool CB_DecideEnemiesInHitbox(
        DamageElement dmgEl,
        TargetInfo target,
        out List<GameObject> list
    )
    {
        return HitboxUtils.GetEnemiesInSphereHitbox(
            out list,
            dmgEl.transform.position,
            this.DecideHitboxRadius(),
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
        return 3f;
    }

    protected override Vector3 DecideInstantiatePosition(TargetInfo target)
    {
        return this.transform.position;
    }

    protected override Quaternion DecideInstantiateRotation(TargetInfo target)
    {
        return this.FaceTarget(target);
    }

    protected override float DecideMaxHitCount()
    {
        return Mathf.Infinity;
    }

    protected override float DecideMoveSpeed()
    {
        return 1f;
    }

    protected override void CB_DecideOnDmg(int hitCount, GameObject enemy)
    {
        // Do nothing
    }

    protected override List<GameObject> DecidePickEnemiesInSight(List<GameObject> sortedEnemies)
    {
        return sortedEnemies;
    }

    protected override bool CB_DecideReadyToDmg(DamageElement dmgEl, TargetInfo target)
    {
        float distance = Vector3.Distance(dmgEl.transform.position, target.GetTarget());
        if (distance <= Mathf.Sqrt(this.DecideHitboxRadius()) / 2)
            return true;
        else
            return false;
    }

    protected override bool DecideShouldTrackDamageEl(DamageElement dmgEl)
    {
        return false;
    }

    protected override List<TargetInfo> DecideTargets(List<GameObject> toAtk)
    {
        return new List<TargetInfo>() { new TargetInfo(toAtk[0]) };
    }
}
