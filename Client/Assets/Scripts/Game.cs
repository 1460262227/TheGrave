using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using Nova;

public class Game : MonoBehaviour {

    public Ground Ground = null;
    public OpShape OpShape = null;
    public Triggers Triggers = null;
    public BloodyNum BloodyNum = null;

    StateMachineManager SMMgr = new StateMachineManager();

    // Use this for initialization
    void Start () {

        Init();

        int w = 10;
        int h = 10;
        Ground.ReCreateGound(w, h);
        Triggers.ReCreateTriggers(2, w, h);
        OpShape.GenNextShape();

        Ground.Player.ID = "player";
        Ground.Player.StartAI("WalkOrAttack");
    }

    void Init()
    {
        AIExt.Ground = Ground;
        AIExt.PathFinder = Ground.PathFinder;
        AIExt.SMMgr = SMMgr;
        AIExt.GetActorAtPos = Ground.GetActorsAtPos;
        AIExt.OnActorCollid += (fromA, toA) => { fromA.OnCollid(toA); toA.OnCollid(fromA); };
        AIExt.BloodyNum = BloodyNum;
        Actor.DestroyActor = Ground.DestroyActor;

        var p = Ground.Player;
        p.SightRange = 3;
        p.AttackRange = 3;
        p.Hp = 10;
        // p.DebugInfo = Debug.Log;
    }
	
	// Update is called once per frame
	void Update () {
        SMMgr.OnTimeElapsed(Time.deltaTime);
    }
}
