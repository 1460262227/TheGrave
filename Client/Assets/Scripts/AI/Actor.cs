using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class Actor : MonoBehaviour {

    public Pos Pos { get; set; }
    public int SightRange { get; set; }
    public int AttackRange { get; set; }
    public string ID { get; set; }
    public List<Pos> MovePath { get; set; }

    public int Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp < 0)
                hp = 0;
        }
    } int hp = 0;

    public bool IsDead()
    {
        return hp <= 0;
    }

    public bool InSightRange(Actor tar)
    {
        return Pos.Dist(tar.Pos) <= SightRange;
    }

    public bool InAttackRange(Actor tar)
    {
        return Pos.Dist(tar.Pos) <= AttackRange;
    }
    
}
