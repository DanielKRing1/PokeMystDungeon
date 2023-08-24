using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehavior : Behavior
{
    public enum MoveIncentive {
        WaitForLeader,
        FollowLeader,
        ChaseEnemy,
        FleeEnemy,
        Roam,
    };

    private RoamHelper rh;

    protected void Start()
    {
        this.rh = new RoamHelper();
    }

    public override void Execute(Observations obs)
    {
        this.Move(obs);
        this.Rotate();
    }

    protected void Move(Observations obs) {
        float speed = this.GetStats().Speed;
        Vector3 dir = this.GetMoveDirection(obs);

        this.GetComponent<Rigidbody>().velocity = speed * dir;
    }

    protected void Rotate() {
        Vector3 dir = this.GetComponent<Rigidbody>().velocity.normalized;

        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 40f
        );
    }

    protected virtual Vector3 GetMoveDirection(Observations obs)
    {
        LeadershipManager lm = this.GetComponent<LeadershipManager>();
        List<GameObject> nearbySorted = ObservationsUtils.SortEnemiesByDistance(
            this.gameObject,
            obs
        );

        switch(this.GetMoveIncentive(nearbySorted)) {
            case MoveIncentive.WaitForLeader:
                return Vector3.zero;
            case MoveIncentive.FollowLeader:
                return (lm.pubSub.GetPub().Me.transform.position - this.transform.position).normalized;
            case MoveIncentive.ChaseEnemy:
                return (nearbySorted[0].transform.position - this.transform.position).normalized;
            case MoveIncentive.FleeEnemy:
                return (this.transform.position - nearbySorted[0].transform.position).normalized;
            case MoveIncentive.Roam:
            default:
                return this.rh.GetRoamDir(this.GetComponent<Rigidbody>().velocity).normalized;
        }
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
    static float ROAM_TIME = 2;

    private Stopwatch roamSW;

    public RoamHelper()
    {
        this.roamSW = new Stopwatch();
    }

    public Vector3 GetRoamDir(Vector3 velocity)
    {
        // 1. Roam timeout has not elapsed -> Keep going in same dir
        if (!this.roamSW.HasElapsed(RoamHelper.ROAM_TIME))
        {
            return velocity.normalized;
        }
        // 2. Roam has timed out -> Roam in new dir within [-90, 90] degrees of original dir
        else
        {
            float curTheta = this.GetCurrentTheta(velocity);
            float nextTheta = this.GetNextTheta(curTheta);

            Vector3 newDir = this.ThetaToVector3(nextTheta).normalized;
            return newDir;
        }
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

    public float GetNextTheta(float curTheta) {
        return (curTheta + Random.Range(-90.0f, 90.0f)) % 360;
    }

    /**
    Get Vector3, given theta in degrees
    */
    private Vector3 ThetaToVector3(float theta)
    {
        return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
    }
}
