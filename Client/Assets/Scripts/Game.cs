using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using Nova;

public class Game : MonoBehaviour {

    public Ground Ground = null;
    public OpShape OpShape = null;
    public Triggers Triggers = null;
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
    }
	
	// Update is called once per frame
	void Update () {
        SMMgr.OnTimeElapsed(Time.deltaTime);
    }
}
