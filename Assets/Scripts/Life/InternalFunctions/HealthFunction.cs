using System;
using UnityEngine;

public class HealthFunction : InternalFunction
{
    private float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        private set { _currentHealth = value; }
    }

    private RecoveryHelper rh;

    public void Awake()
    {
        this.rh = new RecoveryHelper();

        this.CurrentHealth = this.GetStats().Health;
    }

    public override void Execute(Observations obs)
    {
        if (this.HandleDeath())
            return;
        this.HandleRecovery();
    }

    private bool HandleDeath()
    {
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

        this.UpdateHealthBar();
        this.SpawnDmgText(amnt);

        Debug.Log(this + " - " + this.CurrentHealth);
    }

    private void UpdateHealthBar()
    {
        this.GetComponentInChildren<InfoInterface>()
            .UpdateHealthBar(this.CurrentHealth, this.GetStats().Health);
    }

    private void SpawnDmgText(int amnt)
    {
        Vector3 pos = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 1,
            this.transform.position.z
        );
        Quaternion rot = Quaternion.identity;
        GameObject dmgText = Instantiate(Resources.Load("Prefabs/DmgText") as GameObject, pos, rot);

        string prefix = amnt > 0 ? "+" : "";
        dmgText.transform.GetChild(0).GetComponent<TextMesh>().text = prefix + amnt;
        // dmgText.GetComponent<TextMesh>().text = prefix + amnt;
    }

    private void HandleRecovery()
    {
        // Has Health left to recover + Enough time has elapsed -> Heal
        if (this.CurrentHealth < this.GetStats().Health && this.rh.CanRecover())
        {
            Debug.Log("Recovering");
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
        Func<PubSub<LeadershipManager>, bool> rmSub = (PubSub<LeadershipManager> pub) =>
        {
            pub.RmSub(lm.pubSub);

            return false;
        };
        Func<PubSub<LeadershipManager>, bool> rmPub = (PubSub<LeadershipManager> sub) =>
        {
            sub.RmPub(lm.pubSub);

            return false;
        };
        lm.pubSub.StartBroadcastDownstream(rmPub);
        lm.pubSub.StartBroadcastUpstream(rmSub);

        // Broadcast all the way down
        Func<PubSub<LeadershipManager>, bool> updateLeadershipIndicator = (
            PubSub<LeadershipManager> sub
        ) =>
        {
            sub.Me.gameObject.GetComponent<VisualsManager>().ApplyRootLeaderColor();

            return true;
        };
        lm.pubSub.StartBroadcastDownstream(updateLeadershipIndicator);

        this.GetComponent<DestroyManager>().Destroy();
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
        if (this.LastHurtSW.HasElapsed(2) && this.LastRecoveredSW.HasElapsedStart(1))
        {
            return true;
        }

        return false;
    }
}
