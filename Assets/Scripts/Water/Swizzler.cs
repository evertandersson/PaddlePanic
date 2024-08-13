using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Swizzler
{
    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2 (v.x, v.y);
    }
    public static Vector2 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 x0z(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
}
