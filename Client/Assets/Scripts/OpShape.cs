using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OpShape : MonoBehaviour, IDragHandler, IEndDragHandler {

    public Action<int, int, int[]> OnDragging = null;
    public Action OnEndDrag = null;
    public GameObject[] ShapeBlocks = null;

    private void Awake()
    {
        CreateShapes();
    }

    int n;
    int m;
    public void GenNextShape()
    {
        n = Utils.RandNext(0, Shapes.Length);
        m = Utils.RandNext(0, Shapes[n].Length);
        ShowShape();
    }

    public void ShapeChange()
    {
        m = (m + 1) % Shapes[n].Length;
        ShowShape();
    }

    void ShowShape()
    {
        var xys = Shapes[n][m];
        foreach (var b in ShapeBlocks)
            b.SetActive(false);

        Utils.For(xys.Length / 2, (n) =>
        {
            var x = xys[n * 2];
            var y = xys[n * 2 + 1];
            ShapeBlocks[y * 3 + x].SetActive(true);
        });
    }

    #region shapes data

    void CreateShapes()
    {
        var nums = ShapesStr.Length;
        Shapes = new int[nums][][];
        Utils.For(nums, (n) =>
        {
            var changes = ShapesStr[n].Length / 3;
            Shapes[n] = new int[changes][];
            Utils.For(changes, (m) =>
            {
                var xys = new List<int>();
                Utils.For(3, 3, (y, x) =>
                {
                    if (ShapesStr[n][m * 3 + y][x] == '*')
                    {
                        xys.Add(x);
                        xys.Add(y);
                    }
                });
                
                Shapes[n][m] = xys.ToArray();
            });
        });
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        var xys = Shapes[n][m].Clone() as int[];
        var pt = eventData.position;
        OnDragging.SC((int)pt.x, (int)pt.y, xys);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag.SC();
    }

    int[][][] Shapes = null;
    string[][] ShapesStr = new string[][]
    {
        new string[]
        {
            "   ",
            " * ",
            "   ",
        },

        new string[]
        {
            "   ",
            "***",
            "   ",

            " * ",
            " * ",
            " * ",
        },

        new string[]
        {
            "***",
            "* *",
            "***",
        },

        new string[]
        {
            "** ",
            " * ",
            " **",

            "  *",
            "***",
            "*  ",
        },

        new string[]
        {
            " **",
            " * ",
            "** ",

            "*  ",
            "***",
            "  *",
        },

        new string[]
        {
            "*  ",
            "*  ",
            "***",

            "***",
            "*  ",
            "*  ",

            "***",
            "  *",
            "  *",

            "  *",
            "  *",
            "***",
        },

        new string[]
        {
            "  *",
            "  *",
            "***",

            "*  ",
            "*  ",
            "***",

            "***",
            "*  ",
            "*  ",

            "***",
            "  *",
            "  *",
        },

        new string[]
        {
            " * ",
            " * ",
            "***",

            "*  ",
            "***",
            "*  ",

            "***",
            " * ",
            " * ",

            "  *",
            "***",
            "  *",
        }
    };

    #endregion
}
