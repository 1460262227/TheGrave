using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpShape : MonoBehaviour {

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
            ShapeBlocks[y * 4 + x].SetActive(true);
        });
    }

    #region shapes data

    void CreateShapes()
    {
        var nums = ShapesStr.Length;
        Shapes = new int[nums][][];
        Utils.For(nums, (n) =>
        {
            var changes = ShapesStr[n].Length / 4;
            Shapes[n] = new int[changes][];
            Utils.For(changes, (m) =>
            {
                var xys = new List<int>();
                Utils.For(4, 4, (y, x) =>
                {
                    if (ShapesStr[n][m * 4 + y][x] == '*')
                    {
                        xys.Add(x);
                        xys.Add(y);
                    }
                });

                Debug.Assert(xys.Count == 8);
                Shapes[n][m] = xys.ToArray();
            });
        });
    }

    int[][][] Shapes = null;
    string[][] ShapesStr = new string[][]
    {
        new string[]
        {
            "    ",
            "    ",
            "****",
            "    ",

            " *  ",
            " *  ",
            " *  ",
            " *  ",
        },

        new string[]
        {
            "    ",
            " ** ",
            " ** ",
            "    ",
        },

        new string[]
        {
            "    ",
            "**  ",
            " ** ",
            "    ",

            "  * ",
            " ** ",
            " *  ",
            "    ",
        },

        new string[]
        {
            "    ",
            "  **",
            " ** ",
            "    ",

            " *  ",
            " ** ",
            "  * ",
            "    ",
        },

        new string[]
        {
            "    ",
            "*   ",
            "*** ",
            "    ",

            " ** ",
            " *  ",
            " *  ",
            "    ",

            "    ",
            "*** ",
            "  * ",
            "    ",

            "  * ",
            "  * ",
            " ** ",
            "    ",
        },

        new string[]
        {
            "    ",
            "  * ",
            "*** ",
            "    ",

            " *  ",
            " *  ",
            " ** ",
            "    ",

            "    ",
            "*** ",
            "*   ",
            "    ",

            " ** ",
            "  * ",
            "  * ",
            "    ",
        },

        new string[]
        {
            "    ",
            " *  ",
            "*** ",
            "    ",

            " *  ",
            " ** ",
            " *  ",
            "    ",

            "    ",
            "*** ",
            " *  ",
            "    ",

            "  * ",
            " ** ",
            "  * ",
            "    ",
        }
    };

    #endregion
}
