using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehavior : Behavior
{
    public enum MoveIncentive
    {
        WaitForLeader,
        FollowLeader,
        ChaseEnemy,
        FleeEnemy,
        Roam,
    };

    private RoamHelper rh;

    public virtual void Awake()
    {
        this.rh = new RoamHelper();

        this.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public virtual void Start() { }

    public override void Execute(Observations obs)
    {
        this.Move(obs);

        try
        {
            StopCoroutine(this.Rotate());
            StartCoroutine(this.Rotate());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    protected void Move(Observations obs)
    {
        float speed = this.GetStats().Speed;
        Vector3 dir = this.GetMoveDirection(obs);

        this.GetComponent<Rigidbody>().velocity = speed * dir;
    }

    protected IEnumerator Rotate()
    {
        Vector3 moveDir = this.GetComponent<Rigidbody>().velocity.normalized;
        Quaternion targetRot = Quaternion.LookRotation(moveDir);

        Quaternion origRot = transform.rotation;
        // float step = Mathf.PI / 180 * (targetRot.eulerAngles.y - this.transform.eulerAngles.y) / 60;

        float startRot = this.transform.eulerAngles.y;
        float endRot = Quaternion.LookRotation(moveDir).eulerAngles.y;
        float t = 0.0f;
        float DURATION = 0.5f;

        // while (!Mathf.Approximately(this.transform.eulerAngles.y, targetRot.eulerAngles.y))
        while (t < DURATION && Mathf.Abs(this.transform.eulerAngles.y - endRot) > 2f)
        {
            t += Time.deltaTime;

            // float yRotation = Mathf.Lerp(startRot, endRot, t / 1f);
            // this.transform.eulerAngles = new Vector3(
            //     transform.eulerAngles.x,
            //     yRotation,
            //     transform.eulerAngles.z
            // );

            this.transform.rotation = Quaternion.Slerp(origRot, targetRot, t / DURATION);

            // this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);

            // transform.rotation = Quaternion.RotateTowards(origRot, targetRot, step);

            // transform.rotation = Quaternion.RotateTowards(
            //     transform.rotation,
            //     targetRot,
            //     1000 * Time.deltaTime
            // );

            yield return null;
            // step += (1 / 60);
            // yield return new WaitForSeconds(1 / 60);
        }
    }

    protected virtual Vector3 GetMoveDirection(Observations obs)
    {
        LeadershipManager lm = this.GetComponent<LeadershipManager>();
        List<GameObject> nearbySorted = ObservationsUtils.SortEnemiesByDistance(
            this.gameObject,
            obs
        );

        Vector3 dir;
        MoveIncentive mi = this.GetMoveIncentive(nearbySorted);
        switch (mi)
        {
            case MoveIncentive.WaitForLeader:
                dir = Vector3.zero;
                break;
            case MoveIncentive.FollowLeader:
                dir = lm.pubSub.GetPub().Me.transform.position - this.transform.position;
                break;
            case MoveIncentive.ChaseEnemy:
                dir = nearbySorted[0].transform.position - this.transform.position;
                break;
            case MoveIncentive.FleeEnemy:
                dir = this.transform.position - nearbySorted[0].transform.position;
                break;
            case MoveIncentive.Roam:
            default:
                Vector3 vel = this.GetComponent<Rigidbody>().velocity;
                dir = this.rh.GetRoamDir(new Vector3(vel.x, 0, vel.z));
                break;
        }

        return new Vector3(dir.x, 0, dir.z).normalized;
    }

    public MoveIncentive GetMoveIncentive(List<GameObject> nearbySorted)
    {
        LeadershipManager lm = this.GetComponent<LeadershipManager>();

        // 1. Has Leader
        if (lm.pubSub.HasPub())
        {
            // 1.1. Already close to Leader
            if (
                Vector3.Distance(lm.pubSub.GetPub().Me.transform.position, this.transform.position)
                <= this.GetStats().Vision / 2
            )
                return MoveIncentive.WaitForLeader;

            // 1.2. Not close to Leader -> Move toward Leader
            return MoveIncentive.FollowLeader;
        }
        // 2. No Leader
        else
        {
            // 2.1. Has nearby enemy
            if (nearbySorted.Count > 0)
                return MoveIncentive.ChaseEnemy;
            // 2.2. No nearby enemy
            return MoveIncentive.Roam;
        }
    }
}

public class RoamHelper
{
    static readonly float MAX_ROAM_DURATION = 4;

    private Stopwatch roamSW;
    private float roamDur;

    public RoamHelper()
    {
        this.roamSW = new Stopwatch();
        this.ResetTimer();
    }

    public Vector3 GetRoamDir(Vector3 velocity)
    {
        // 1. Roam timeout has not elapsed -> Keep going in same dir
        if (!this.roamSW.HasElapsed(this.roamDur))
            return velocity.normalized;

        // 2. Roam has timed out -> Roam in new dir within [-90, 90] degrees of original dir
        float curTheta = this.GetCurrentTheta(velocity);
        float nextTheta = this.GetNextTheta(curTheta);

        Vector3 newDir = this.ThetaToVector3(nextTheta).normalized;

        this.ResetTimer();
        return newDir;
    }

    private void ResetTimer()
    {
        this.roamDur = RandUtils.GetRandFloat(MAX_ROAM_DURATION / 2, MAX_ROAM_DURATION);
        this.roamSW.Start();
    }

    /**
    Get theta of given vector, in degrees
    */
    public float GetCurrentTheta(Vector3 velocity)
    {
        float x = velocity.x;
        float z = velocity.z;

        float theta = Mathf.Atan2(x, z) * Mathf.Rad2Deg;

        return theta;
    }

    public float GetNextTheta(float curTheta)
    {
        return (curTheta + RandUtils.GetRandFloat(-90.0f, 90.0f)) % 360;
    }

    /**
    Get Vector3, given theta in degrees
    */
    private Vector3 ThetaToVector3(float theta)
    {
        return new Vector3(Mathf.Cos(theta * Mathf.PI / 180), 0, Mathf.Sin(theta * Mathf.PI / 180));
    }
}
