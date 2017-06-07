using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;

public class Diamond : Actor
{
    public override void StartAt(Pos pos)
    {
        base.StartAt(pos);
        this.StartAI("StayAndCollid");
    }

    public override void OnCollid(Actor a)
    {
        if (!(a is Player))
            return;

        var p = a as Player;
        p.Starts++;

        DestroyActor(this);
    }
}
