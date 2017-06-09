using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

    public int Starts { get; set; }

    public override bool IsEnemy(Actor target)
    {
        return target is Monster;
    }

    public override int Hp
    {
        get
        {
            return base.Hp;
        }

        set
        {
            base.Hp = value;
            if (IsDead())
            {
                var rs = GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in rs)
                    r.color = new Color(r.color.r, r.color.g, r.color.b, 0.5f);
            }
        }
    }
}
