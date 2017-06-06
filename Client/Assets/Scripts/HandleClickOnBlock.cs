using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Nova;

public class HandleClickOnBlock : MonoBehaviour, IPointerClickHandler {

    public Ground Ground = null;
    public OpShape OpShape = null;
    public Player Player = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        int bx;
        int by;
        var pt = eventData.position;
        if (Ground.ToBlockPos(pt.x, pt.y, out bx, out by))
        {
            var path = Player.FindPath(new Pos(bx, by));
            if (path.Count == 0)
                return;

            Player.MoveOnPath(path);
        }
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
