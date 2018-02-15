using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class SwapExtension
{
    public static T Swap<T>(this T x, ref T y)
    {
        T t = y;
        y = x;
        return t;
    }
}
