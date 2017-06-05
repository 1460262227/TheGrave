using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger
{
    void UncoverAt(int x, int y);
}

public class Triggers : MonoBehaviour {

    public Diamond Diamond = null;
    public Monster Monster = null;
    
    int w;
    int h;
    int l;
    ITrigger[][,] triggers = null;
    List<GameObject> allocated = new List<GameObject>();
    public void ReCreateTriggers(int layers, int width, int height)
    {
        DestroyAll();

        w = width;
        h = height;
        l = layers;

        triggers = new ITrigger[l][,];
        Utils.For(layers, (n) =>
        {
            triggers[n] = new ITrigger[w, h];
            Utils.For(w, h, (x, y) =>
            {
                var t = GenRandomTrigger();
                triggers[n][x, y] = t;
                if (t != null)
                    allocated.Add((t as MonoBehaviour).gameObject);
            });
        });
    }

    public ITrigger Uncover(int layer, int x, int y)
    {
        var t = triggers[layer][x, y];
        triggers[layer][x, y] = null;
        if (t != null)
            t.UncoverAt(x, y);
        return t;
    }

    void DestroyAll()
    {
        foreach (var t in allocated)
            Destroy(t);

        allocated.Clear();
    }

    ITrigger GenRandomTrigger()
    {
        if (Utils.Hit(0.05f))
            return Instantiate(Diamond);
        else if (Utils.Hit(0.1f))
            return Instantiate(Monster);
        else
            return null;
    }
}
