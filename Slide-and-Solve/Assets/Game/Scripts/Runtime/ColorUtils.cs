using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static Color MoveTowards(this Color a, Color b, float delta) {
        return new Color(
            Mathf.MoveTowards(a.r, b.r, delta),
            Mathf.MoveTowards(a.g, b.g, delta),
            Mathf.MoveTowards(a.b, b.b, delta),
            Mathf.MoveTowards(a.a, b.a, delta));
    }
}
