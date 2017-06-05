using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HandleClickOnBlock : MonoBehaviour {

    public Ground Ground = null;
    public Action<int, int> OnClickGorund = null;
    public OpShape OpShape = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        int bx;
        int by;
        var pt = eventData.position;
        if (Ground.ToBlockPos(pt.x, pt.y, out bx, out by))
            OnClickGorund.SC(bx, by);
    }

    private void Start()
    {
        OpShape.OnDragging += (sx, sy, xys) =>
        {
            int bx;
            int by;
            Ground.ToBlockPos(sx, sy, out bx, out by);
            Utils.For(xys.Length / 2, (n) =>
            {
                var x = xys[n * 2] + bx;
                var y = xys[n * 2 + 1] + by;
                xys[n * 2] = x;
                xys[n * 2 + 1] = y;
            });

            if (!Ground.Valid(xys))
                return;

            Ground.Selected = xys;
        };

        OpShape.OnEndDrag += () =>
        {
            if (Ground.FillSelected())
                OpShape.GenNextShape();
        };
    }
}
