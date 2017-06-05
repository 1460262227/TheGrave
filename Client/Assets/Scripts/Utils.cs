using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils{
    
    public static void For(int start, int end, Action<int> func)
    {
        for (int i = start; i < end; i++)
            func(i);
    }

    public static void For(int end, Action<int> func)
    {
        For(0, end, func);
    }

    public static void For(int start1, int end1, int start2, int end2, Action<int, int> func)
    {
        For(start1, end1, (a) => { For(start2, end2, (b) => { func(a, b); }); });
    }

    public static void For(int end1, int end2, Action<int, int> func)
    {
        For(0, end1, 0, end2, func);
    }

    public static void SC(this Action call)
    {
        if (call != null)
            call();
    }

    public static void SC<T>(this Action<T> call, T p)
    {
        if (call != null)
            call(p);
    }

    public static void SC<T1, T2>(this Action<T1, T2> call, T1 p1, T2 p2)
    {
        if (call != null)
            call(p1, p2);
    }

    static System.Random rand = new System.Random();
    public static int RandNext(int min, int max)
    {
        return rand.Next(min, max);
    }
}
