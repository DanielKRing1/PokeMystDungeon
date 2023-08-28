using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAttack : AttackBehavior
{
    protected override void CB_DecideAfterExecute(DamageElement dmgEl)
    {
        // Do nothing
    }

    protected override string DecideDmgElResourcePath()
    {
        return "Prefabs/DmgEl";

    }

    protected override float DecideBaseCooldown()
    {
        return 1f;
    }

    protected override float DecideDmgMultiplier()
    {
        return 1f;
    }

    protected override bool CB_DecideEnemiesInHitbox(TargetInfo target, out List<GameObject> list)
    {
        return HitboxUtils.GetEnemiesInSphereHitbox(
            out list,
            this.transform.position,
            this.DecideHitboxRadius(),
            this.GetComponent<LeadershipManager>().GetRootLeader());
    }

    protected override bool CB_DecideHandleIfReachTarget(DamageElement dmgEl, TargetInfo target)
    {
        return true;
    }

    protected override float DecideHitboxRadius()
    {
        return 0.5f;
    }

    protected override Vector3 DecideInstantiatePosition(TargetInfo target)
    {
        return this.transform.position;
    }

    protected override Quaternion DecideInstantiateRotation(TargetInfo target)
    {
        return this.transform.rotation;
    }

    protected override float DecideMaxHitCount()
    {
        return 1;
    }

    protected override float DecideMoveSpeed()
    {
        return 2f;
    }

    protected override void CB_DecideOnDmg(int hitCount, GameObject enemy)
    {
        // Do nothing
    }

    protected override List<GameObject> DecidePickEnemiesInSight(List<GameObject> sortedEnemies)
    {
        return sortedEnemies;
    }

    protected override bool CB_DecideReadyToDmg(DamageElement dmgEl, TargetInfo ti)
    {
        return true;
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
