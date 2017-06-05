using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Ground Ground = null;
    public OpShape OpShape = null;
    public Triggers Triggers = null;

    // Use this for initialization
    void Start () {
        int w = 10;
        int h = 10;
        Ground.ReCreateGound(w, h);
        Triggers.ReCreateTriggers(2, w, h);
        OpShape.GenNextShape();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
