using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFunction : InternalFunction
{
    private int _currentHealth;
    public int CurrentHealth
    {
        get { return _currentHealth; }
        private set { _currentHealth = value; }
    }

    private RecoveryHelper rh;
    
    private void Start() {
        this.rh = new RecoveryHelper();
    }

    public override void Execute(Observations obs)
    {

        if(this.HandleDeath()) return;
        this.HandleRecovery();
    }

    private bool HandleDeath() {
        if (this.IsDead())
        {
            this.Die();
            return true;
        }

        return false;
    }

    /**
    Return true if killed, else false
    */
    public bool Hurt(int dmg)
    {
        // 1. Reduce incoming damage with defense
        dmg = (int)(dmg / Mathf.Log(this.GetStats().Defense));

        // 2. Apply damage
        this.AlterHealth(-dmg);

        // 3. Record 'Hurt'
        this.rh.RecordHurt();

        // 4. Return whether dead
        return this.IsDead();
    }

    public void Heal(int heal)
    {
        this.AlterHealth(heal);
    }

    /**
    Pass a positive value to Heal,
    negative to Hurt

    Clamps between 0 and max health
    */
    private void AlterHealth(int amnt)
    {
        this.CurrentHealth = Mathf.Clamp(this.CurrentHealth + amnt, 0, (int)this.GetStats().Health);

        this.SpawnDmgText(amnt);
    }

    private void SpawnDmgText(int amnt)
    {
        GameObject dmgText = Instantiate(
            Resources.Load("Prefabs/DmgText") as GameObject
        );

        string prefix =
            amnt > 0
                ? "+"
                : amnt < 0
                    ? "-"
                    : "";
        dmgText.transform.GetChild(0).GetComponent<TextMesh>().text = prefix + amnt;
        // dmgText.GetComponent<TextMesh>().text = prefix + amnt;
    }

    private void HandleRecovery()
    {
        // Has Health left to recover + Enough time has elapsed -> Heal
        if (this.CurrentHealth < this.GetStats().Health && this.rh.CanRecover())
        {
            this.Heal((int)this.GetStats().Health / 10);
        }
    }

    private bool IsDead()
    {
        return this.CurrentHealth <= 0;
    }

    private void Die()
    {
        LeadershipManager lm = this.gameObject.GetComponent<LeadershipManager>();

        // Broadcast up and down to Rm this PubSub from immediate Leader/Followers
        Func<PubSub<LeadershipManager>, bool> rmSub = (PubSub<LeadershipManager> pub) => {
            pub.RmSub(lm.pubSub);

            return false;
        };
        Func<PubSub<LeadershipManager>, bool> rmPub = (PubSub<LeadershipManager> sub) => {
            sub.RmPub(lm.pubSub);

            return false;
        };
        lm.pubSub.StartBroadcastDownstream(rmPub);
        lm.pubSub.StartBroadcastUpstream(rmSub);

        // Broadcast all the way down
        Func<PubSub<LeadershipManager>, bool> updateLeadershipIndicator = (PubSub<LeadershipManager> sub) => {
            sub.Me.ApplyLeaderIndicatorColor();

            return true;
        };
        lm.pubSub.StartBroadcastDownstream(updateLeadershipIndicator);

        Destroy(this.gameObject);
    }
}

public class RecoveryHelper
{
    private Stopwatch LastHurtSW;
    private Stopwatch LastRecoveredSW;

    public RecoveryHelper()
    {
        LastHurtSW = new Stopwatch();
        LastRecoveredSW = new Stopwatch();
    }

    public void RecordHurt()
    {
        this.LastHurtSW.Start();
    }

    public bool CanRecover()
    {
        // 1. Recover Health until at least 2 seconds after last hurt + Wait every second
        if (this.LastHurtSW.HasElapsed(2) && this.LastRecoveredSW.HasElapsed(1))
        {
            this.LastRecoveredSW.Start();

            return true;
        }

        return false;
    }
}
