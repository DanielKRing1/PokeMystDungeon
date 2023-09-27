using System;
using System.Collections.Generic;

public abstract class PubSub<T>
{
    public readonly T Me;

    private PubSub<T> _directPub;
    public PubSub<T> DirectPub
    {
        get { return _directPub; }
        private set { _directPub = value; }
    }

    protected HashSet<PubSub<T>> DirectSubs;

    public PubSub(T me, PubSub<T> pub)
    {
        this.Me = me;
        this.DirectPub = pub;
        this.DirectSubs = new HashSet<PubSub<T>>();
    }

    // ADD PUB/SUB

    protected bool _ValidatePub(PubSub<T> pub)
    {
        // 1. Pub is not already an upstream Pub and
        HashSet<PubSub<T>> allPubs = new HashSet<PubSub<T>>();
        this.GetAllPubs(allPubs);

        // 2. Further validation
        return !allPubs.Contains(pub) && this.ValidatePub(pub);
    }

    protected abstract bool ValidatePub(PubSub<T> pub);

    // this._ValidatePub/Sub is very important to prevent infinite loop
    // -> If pub/sub is already registered, will short-circuit to prevent infinite loop
    public bool AddPub(PubSub<T> pub)
    {
        if (!this._ValidatePub(pub))
            return false;

        this.DirectPub = pub;
        pub.AddSub(this);

        return true;
    }

    protected bool _ValidateSub(PubSub<T> sub)
    {
        // 1. Sub is not already a downstream Sub and My Leadership must scale above Sub Leadership
        HashSet<PubSub<T>> allSubs = new HashSet<PubSub<T>>();
        this.GetAllSubs(allSubs);

        return !allSubs.Contains(sub) && this.ValidateSub(sub);
    }

    protected abstract bool ValidateSub(PubSub<T> sub);

    // this._ValidatePub/Sub is very important to prevent infinite loop
    // -> If pub/sub is already registered, will short-circuit to prevent infinite loop
    public bool AddSub(PubSub<T> sub)
    {
        if (!this._ValidateSub(sub))
            return false;

        this.DirectSubs.Add(sub);
        sub.AddPub(this);

        return true;
    }

    // REMOVE PUB/SUB

    public bool RmPub(PubSub<T> pub)
    {
        if (this.GetPub() == pub)
        {
            this.DirectPub = this;
            return true;
        }

        return false;
    }

    public bool RmSub(PubSub<T> sub)
    {
        if (this.DirectSubs.Contains(sub))
        {
            this.DirectSubs.Remove(sub);
            return true;
        }

        return false;
    }

    // GET ALL SUBS/PUBS

    public void GetAllSubs(HashSet<PubSub<T>> allSubs)
    {
        StartBroadcastDownstream(
            (PubSub<T> sub) =>
            {
                allSubs.Add(sub);
                return true;
            }
        );
    }

    public void GetAllPubs(HashSet<PubSub<T>> allPubs)
    {
        StartBroadcastUpstream(
            (PubSub<T> pub) =>
            {
                allPubs.Add(pub);
                return true;
            }
        );
    }

    // BROADCAST

    public void StartBroadcastDownstream(Func<PubSub<T>, bool> func)
    {
        foreach (PubSub<T> ps in this.DirectSubs)
        {
            bool continueBroadcast = func(ps);

            if (continueBroadcast)
                ps.StartBroadcastDownstream(func);
        }
    }

    public void StartBroadcastUpstream(Func<PubSub<T>, bool> func)
    {
        if (!this.HasPub())
            return;

        bool continueBroadcast = func(this.GetPub());

        if (continueBroadcast)
            this.GetPub().StartBroadcastUpstream(func);
    }

    // UTILS

    public bool HasPub()
    {
        return this.GetPub() != this;
    }

    public PubSub<T> GetPub()
    {
        if (this.DirectPub == null)
            this.DirectPub = this;

        return this.DirectPub;
    }

    /**
    Returns the top-most DirectPub, ie the DirectPub of this DirectPub's DirectPub's ... DirectPub
    */
    public PubSub<T> GetRootPub()
    {
        // 1. Get top-most Leader (dig until Leader is self)
        PubSub<T> pub = this.GetPub();
        while (pub.HasPub())
        {
            pub = pub.GetPub();
        }

        return pub;
    }
}
