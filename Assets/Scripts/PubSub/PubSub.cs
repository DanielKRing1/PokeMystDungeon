using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PubSub<T>
{
    public readonly T Me;

    private PubSub<T> _pub;
    public PubSub<T> Pub
    {
        get { return _pub; }
        private set { _pub = value; }
    }

    protected HashSet<PubSub<T>> subs;

    public PubSub(T me, PubSub<T> pub)
    {
        this.Me = me;
        this.Pub = pub;
        this.subs = new HashSet<PubSub<T>>();
    }

    // ADD PUB/SUB

    protected bool _ValidatePub(PubSub<T> pub)
    {
        return this.Pub != pub && this.ValidatePub(pub);
    }

    protected abstract bool ValidatePub(PubSub<T> pub);

    // this._ValidatePub/Sub is very important to prevent infinite loop
    // -> If pub/sub is already registered, will short-circuit to prevent infinite loop
    public bool AddPub(PubSub<T> pub)
    {
        if (!this._ValidatePub(pub))
            return false;

        this.Pub = pub;
        pub.AddSub(this);

        return true;
    }

    protected bool _ValidateSub(PubSub<T> sub)
    {
        return !this.subs.Contains(sub) && this.ValidateSub(sub);
    }

    protected abstract bool ValidateSub(PubSub<T> sub);

    // this._ValidatePub/Sub is very important to prevent infinite loop
    // -> If pub/sub is already registered, will short-circuit to prevent infinite loop
    public bool AddSub(PubSub<T> sub)
    {
        if (!this._ValidateSub(sub))
            return false;

        this.subs.Add(sub);
        sub.AddPub(this);

        return true;
    }

    // REMOVE PUB/SUB

    public bool RmPub(PubSub<T> pub)
    {
        if (this.GetPub() == pub)
        {
            this.Pub = this;
            return true;
        }

        return false;
    }

    public bool RmSub(PubSub<T> sub)
    {
        if (this.subs.Contains(sub))
        {
            this.subs.Remove(sub);
            return true;
        }

        return false;
    }

    // BROADCAST

    public void StartBroadcastDownstream(Func<PubSub<T>, bool> func)
    {
        foreach (PubSub<T> ps in this.subs)
        {
            ps.BroadcastDownstream(func);
        }
    }

    public void BroadcastDownstream(Func<PubSub<T>, bool> func)
    {
        bool continueBroadcast = func(this);

        if (continueBroadcast)
            this.StartBroadcastDownstream(func);
    }

    public void StartBroadcastUpstream(Func<PubSub<T>, bool> func)
    {
        if (!this.HasPub())
            return;
        this.GetPub().BroadcastUpstream(func);
    }

    public void BroadcastUpstream(Func<PubSub<T>, bool> func)
    {
        bool continueBroadcast = func(this);

        if (continueBroadcast)
            this.StartBroadcastUpstream(func);
    }

    // UTILS

    public bool HasPub()
    {
        return this.GetPub() != this;
    }

    public PubSub<T> GetPub()
    {
        if (this.Pub == null)
            this.Pub = this;

        return this.Pub;
    }

    /**
    Returns the top-most Pub, ie the Pub of this Pub's Pub's ... Pub
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
