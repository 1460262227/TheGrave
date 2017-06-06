using AStar;
using Nova;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Block BlockTemplate = null;
    public Vector2 BlockSize = new Vector2(1, 1);
    public Triggers Triggers = null;
    public Player Player = null;

    Block[,] blocks = null;
    List<Block> allocated = new List<Block>();
    int w;
    int h;
    Vector3 offset = Vector3.zero;

    public AStarPathFinder PathFinder { get { return pathFinder; } }
    AStarPathFinder pathFinder = new AStarPathFinder();

    public bool ToBlockPos(float sx, float sy, out int bx, out int by)
    {
        var cam = Camera.main;
        var wp = cam.ScreenToWorldPoint(new Vector3(sx, sy, 0));
        var bp = new Vector3(wp.x, wp.y, 0) + offset;
        bx = (int)(bp.x / BlockSize.x + 0.5f);
        by = (int)(bp.y / BlockSize.y + 0.5f);
        return bx >= 0 && bx < w && by >= 0 && by < h;
    }

    public Vector3 ToWorldPos(float bx, float by)
    {
        var wp = new Vector3(BlockSize.x * bx, BlockSize.y * by, 0) - offset;
        return wp;
    }

    public int[] Selected
    {
        set
        {
            ClearSelected();
            selected = value;

            if (selected == null)
                return;

            Utils.For(value.Length / 2, (n) =>
            {
                var x = selected[n * 2];
                var y = selected[n * 2 + 1];
                blocks[x, y].Selected = true;
            });
        }
    } int[] selected = null;

    public bool FillSelected()
    {
        if (selected == null)
            return false;

        Utils.For(selected.Length / 2, (n) =>
        {
            var x = selected[n * 2];
            var y = selected[n * 2 + 1];
            blocks[x, y].Layer++;

            Triggers.Uncover(blocks[x, y].Layer - 1, x, y);
            pathFinder.SetHeight(x, y, -blocks[x, y].Layer);
        });

        Selected = null;
        return true;
    }

    public bool Valid(int[] pts)
    {
        bool valid = true;
        int layer = -1;
        Utils.For(pts.Length / 2, (n) =>
        {
            var x = pts[n * 2];
            var y = pts[n * 2 + 1];
            if (x < 0 || x >= w || y < 0 || y >= h)
            {
                valid = false;
                return;
            }

            if (blocks[x, y].Layer == blocks[x, y].Layers.Length - 1)
            {
                valid = false;
                return;
            }

            if (layer == -1)
                layer = blocks[x, y].Layer;
            else if (layer != blocks[x, y].Layer)
            {
                valid = false;
                return;
            }
        });

        return valid;
    }

    void ClearSelected()
    {
        foreach (var b in blocks)
            b.Selected = false;
    }

    public void ReCreateGound(int width, int height)
    {
        DestroyAll();

        w = width;
        h = height;
        blocks = new Block[w, h];
        offset = new Vector3((w - 1) * BlockSize.x / 2, (h - 1) * BlockSize.y / 2, 0);
        Utils.For(w, h, (x, y) =>
        {
            var b = Instantiate(BlockTemplate) as Block;
            b.gameObject.SetActive(true);
            b.Layer = 0;
            b.transform.SetParent(transform, false);
            b.transform.localPosition = ToWorldPos(x, y);
            blocks[x, y] = b;
        });

        pathFinder.ResetMapData(new int[w * h], w, h);

        Player.Move2(0, 0);
    }

    void DestroyAll()
    {
        foreach (var b in allocated)
            Destroy(b);

        allocated.Clear();
    }
}
