﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public GameObject[] Layers = null;
    public GameObject SelectedLayer = null;
    public GameObject InvalidSelectedLayer = null;

    public int Layer
    {
        get { return layer; }
        set
        {
            layer = Utils.Clamp(value, 0, Layers.Length - 1);
            RefreshLayersVisible();
        }
    } int layer = 0;

    void RefreshLayersVisible()
    {
        foreach (var l in Layers)
            l.SetActive(l == Layers[layer]);
    }

    public bool Selected
    {
        set
        {
            SelectedLayer.SetActive(value);
            InvalidSelectedLayer.SetActive(false);
        }
    }

    public bool InvalidSelected
    {
        set
        {
            SelectedLayer.SetActive(false);
            InvalidSelectedLayer.SetActive(value);
        }
    }
}
