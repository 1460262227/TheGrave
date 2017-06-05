using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Block BlockTemplate = null;
    public Vector2 BlockSize = new Vector2(1, 1);

    Block[,] blocks = null;
    List<Block> allocated = new List<Block>();
    int w;
    int h;
    Vector3 offset = Vector3.zero;

    public bool ToBlockPos(float sx, float sy, out int bx, out int by)
    {
        var cam = Camera.main;
        var wp = cam.ScreenToWorldPoint(new Vector3(sx, sy, 0));
        var bp = new Vector3(wp.x, wp.y, 0) + offset;
        bx = (int)(bp.x / BlockSize.x);
        by = (int)(bp.y / BlockSize.y);
        return bx >= 0 && bx < w && by >= 0 && by < h;
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

    public void FillSelected()
    {
        if (selected == null)
            return;

        Utils.For(selected.Length / 2, (n) =>
        {
            var x = selected[n * 2];
            var y = selected[n * 2 + 1];
            blocks[x, y].Layer++;
        });

        Selected = null;
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
            b.transform.position = new Vector3(BlockSize.x * x, BlockSize.y * y, 0) - offset;
            blocks[x, y] = b;
        });
    }

    void DestroyAll()
    {
        foreach (var b in allocated)
            Destroy(b);

        allocated.Clear();
    }
}
