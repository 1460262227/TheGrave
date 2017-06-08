using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

    public int Starts { get; set; }

    public override bool IsEnemy(Actor target)
    {
        return target is Monster;
    }
}
