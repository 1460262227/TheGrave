using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;

public class Monster : Actor
{
    public override bool KeepInLayer { get { return true; } }

    public override void StartAt(Pos pos)
    {
        base.StartAt(pos);
        this.StartAI("PatrolInLayer");
    }

    public override bool IsEnemy(Actor target)
    {
        return target is Player;
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
                gameObject.SetActive(false);
                this.StopAI();
            }
        }
    }
}
