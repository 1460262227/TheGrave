using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Ground Ground = null;
    public OpShape OpShape = null;

	// Use this for initialization
	void Start () {
        Ground.ReCreateGound(13, 13);
        OpShape.GenNextShape();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
