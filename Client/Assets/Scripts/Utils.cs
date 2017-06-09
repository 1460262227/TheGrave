using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils{
    
    public static void For(int start, int end, Action<int> func, Func<bool> exitCondition = null)
    {
        var d = end > start ? 1 : -1;
        for (int i = start; (d > 0 ? (i < end) : (i > end)) && (exitCondition == null || !exitCondition()); i += d)
            func(i);
    }

    public static void For(int end, Action<int> func, Func<bool> exitCondition = null)
    {
        For(0, end, func, exitCondition);
    }

    public static void For(int start1, int end1, int start2, int end2, Action<int, int> func, Func<bool> exitCondition = null)
    {
        For(start1, end1, (a) => { For(start2, end2, (b) => { func(a, b); }, exitCondition); }, exitCondition);
    }

    public static void For(int end1, int end2, Action<int, int> func, Func<bool> exitCondition = null)
    {
        For(0, end1, 0, end2, func, exitCondition);
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

    public static void SC<T1, T2, T3>(this Action<T1, T2, T3> call, T1 p1, T2 p2, T3 p3)
    {
        if (call != null)
            call(p1, p2, p3);
    }

    static System.Random rand = new System.Random();
    public static int RandNext(int min, int max)
    {
        return rand.Next(min, max);
    }

    public static bool Hit(float rate)
    {
        return RandNext(0, int.MaxValue) / (float)(int.MaxValue - 1) < rate;
    }

    public static T Clamp<T>(T v, T min, T max) where T: IComparable
    {
        if (v.CompareTo(min) < 0)
            return min;
        else if (v.CompareTo(max) >= 0)
            return max;
        else
            return v;
    }

    public static string RandomName(string prefix = null)
    {
        var rn = "_" + DateTime.Now.Ticks + "_" + RandNext(10000, 99999);
        return prefix == null ? rn : prefix + rn;
    }

    public static List<int> Range(int start, int end)
    {
        var lst = new List<int>();
        For(start, end, (i) => { lst.Add(i); });
        return lst;
    }

    public static List<int> Range(int end)
    {
        return Range(0, end);
    }

    public static List<T> Disorder<T>(this List<T> lst)
    {
        var cnt = lst.Count;
        For(cnt, (i) =>
        {
            var rn = RandNext(0, cnt);
            var tmp = lst[i];
            lst[i] = lst[rn];
            lst[rn] = tmp;
        });

        return lst;
    }

    public static T[] Disorder<T>(this T[] arr)
    {
        var cnt = arr.Length;
        For(cnt, (i) =>
        {
            var rn = RandNext(0, cnt);
            var tmp = arr[i];
            arr[i] = arr[rn];
            arr[rn] = tmp;
        });

        return arr;
    }
}
