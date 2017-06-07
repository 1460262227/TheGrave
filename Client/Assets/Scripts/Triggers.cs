using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class Triggers : MonoBehaviour {

    public Ground Ground = null;
    public Diamond Diamond = null;
    public Monster Monster = null;
    
    int w;
    int h;
    int l;
    Actor[][,] triggers = null;
    List<GameObject> allocated = new List<GameObject>();
    public void ReCreateTriggers(int layers, int width, int height)
    {
        DestroyAll();

        w = width;
        h = height;
        l = layers;

        triggers = new Actor[l][,];
        Utils.For(layers, (n) =>
        {
            triggers[n] = new Actor[w, h];
            Utils.For(w, h, (x, y) =>
            {
                var t = GenRandomTrigger();
                triggers[n][x, y] = t;
                if (t != null)
                {
                    t.transform.SetParent(transform, false);
                    t.transform.localPosition = Ground.ToWorldPos(x, y);
                    t.gameObject.SetActive(false);
                    allocated.Add(t.gameObject);
                } 
            });
        });
    }

    public Actor Uncover(int layer, int x, int y)
    {
        var t = triggers[layer][x, y];
        triggers[layer][x, y] = null;
        if (t != null)
        {
            t.gameObject.SetActive(true);
            t.StartAt(new Pos(x, y));
        }

        return t;
    }

    void DestroyAll()
    {
        foreach (var t in allocated)
            Destroy(t);

        allocated.Clear();
    }

    Actor GenRandomTrigger()
    {
        Actor a = null;
        if (Utils.Hit(0.05f))
            a = Instantiate(Diamond) as Actor;
        else if (Utils.Hit(0.1f))
            a = Instantiate(Monster) as Actor;

        if (a != null)
            a.ID = Utils.RandomName("Actor");

        return a;
    }
}
