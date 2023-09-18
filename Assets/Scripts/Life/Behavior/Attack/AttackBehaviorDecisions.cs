using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviorDecisions : Behavior
{
    /**
    Define the interval at which this attack executes: Is it generally slow or quick?
    The scaled cooldown will factor in the atkspd stat
    */
    protected abstract float DecideBaseCooldown();

    // GET NEARBY ENEMIES

    /**
    Pick a subset of enemies from a list of sorted enemies
    */
    protected abstract List<GameObject> DecidePickEnemiesInSight(List<GameObject> sortedEnemies);

    // INSTANTIATE DAMAGE ELEMENT

    protected abstract List<TargetInfo> DecideTargets(List<GameObject> toAtk);
    protected abstract Vector3 DecideInstantiatePosition(TargetInfo target);
    protected abstract Quaternion DecideInstantiateRotation(TargetInfo target);

    protected abstract bool DecideShouldTrackDamageEl(DamageElement dmgEl);

    // Example "Prefabs/BasicAttack"
    protected abstract string DecideDmgElResourcePath();

    protected abstract float DecideMoveSpeed();

    protected abstract bool CB_DecideHandleIfReachTarget(DamageElement dmgEl, TargetInfo target);

    // READY TO DMG ----

    protected abstract bool CB_DecideReadyToDmg(DamageElement dmgEl, TargetInfo ti);

    protected abstract float DecideHitboxRadius();
    protected abstract bool CB_DecideEnemiesInHitbox(TargetInfo target, out List<GameObject> list);

    // GET DMG ----

    /**
    Define the damage multiplier for this attack: Is it generally strong or weak?
    The scaled damage will factor in the atk stat
    */
    protected abstract float DecideDmgMultiplier();

    // ON DMG ----

    /**
    Return true if can continue hitting enemies
    */
    protected abstract void CB_DecideOnDmg(int hitCount, GameObject enemy);

    protected abstract float DecideMaxHitCount();

    // AFTER EXECUTE ----

    /**
    Do something, then
    Return true to destroy DamageEl, else false
    */
    protected abstract bool CB_DecideAfterExecute(DamageElement dmgEl);
}
