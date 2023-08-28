using System.Collections.Generic;
using UnityEngine;

public class OrbitAttack : AttackBehavior
{
    private const int DMG_EL_COUNT = 3;
    private const int RADIUS = 3;
    private const float ORBIT_PERIOD = 4f;

    public override void Start()
    {
        base.Start();
        this.InstantiateDamageElements(null);
    }

    public override void Execute(Observations obs)
    {
        // Do nothing
    }

    protected override float DecideBaseCooldown()
    {
        return 1f;
    }

    protected override List<GameObject> DecidePickEnemiesInSight(List<GameObject> sortedEnemies)
    {
        return sortedEnemies;
    }

    protected override List<TargetInfo> DecideTargets(List<GameObject> toAtk)
    {
        List<TargetInfo> targets = new List<TargetInfo>();

        float theta = 0;
        for (int i = 0; i < OrbitAttack.DMG_EL_COUNT; i++)
        {
            Vector3 dir = new Vector3(
                Mathf.Sin(theta / OrbitAttack.RADIUS),
                this.transform.position.y,
                Mathf.Cos(theta / OrbitAttack.RADIUS)
            );
            TargetInfo target = new TargetInfo(this.transform.position + OrbitAttack.RADIUS * dir);
            targets.Add(target);

            theta += (360 / OrbitAttack.DMG_EL_COUNT);
        }

        return targets;
    }

    protected override Vector3 DecideInstantiatePosition(TargetInfo target)
    {
        return target.GetTarget();
    }

    protected override Quaternion DecideInstantiateRotation(TargetInfo target)
    {
        return this.FaceTarget(target);
    }

    protected override bool DecideShouldTrackDamageEl(DamageElement dmgEl)
    {
        return true;
    }

    protected override string DecideDmgElResourcePath()
    {
        return "Prefabs/DmgEl";
    }

    protected override void CB_MovementController(DamageElement dmgEl, TargetInfo ti)
    {
        // 1. Update rotation since last frame
        float yRot = 360 * (Time.deltaTime / OrbitAttack.ORBIT_PERIOD) % 360;
        dmgEl.transform.Rotate(new Vector3(0, yRot, 0));

        // 2. Set position based on rotation
        dmgEl.transform.position =
            this.transform.position + OrbitAttack.RADIUS * dmgEl.transform.forward;
    }

    protected override float DecideMoveSpeed()
    {
        return 0;
    }

    protected override bool CB_DecideHandleIfReachTarget(DamageElement dmgEl, TargetInfo target)
    {
        return false;
    }

    protected override bool CB_DecideReadyToDmg(DamageElement dmgEl, TargetInfo ti)
    {
        return true;
    }

    protected override float DecideHitboxRadius()
    {
        return 1f;
    }

    protected override bool CB_DecideEnemiesInHitbox(TargetInfo target, out List<GameObject> list)
    {
        return HitboxUtils.GetEnemiesInSphereHitbox(
            out list,
            this.transform.position,
            this.DecideHitboxRadius(),
            this.GetComponent<LeadershipManager>().GetRootLeader()
        );
    }

    protected override float DecideDmgMultiplier()
    {
        return 0.8f;
    }

    protected override void CB_DecideOnDmg(int hitCount, GameObject enemy)
    {
        // Do nothing
    }

    protected override float DecideMaxHitCount()
    {
        return Mathf.Infinity;
    }

    protected override bool CB_DecideAfterExecute(DamageElement dmgEl)
    {
        // Do nothing
        return false;
    }
}
