using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlowData 
{
    [SerializeField]
    public Color outerColor = Color.black;

    [SerializeField]
    public Color innerColor = Color.black;

    [SerializeField]
    public float outerThreshold = 0f;

    [SerializeField]
    public float innerThreshold = 0f;

    public GlowData()
    {
    }
    public GlowData(GlowData copy)
    {
        this.outerColor = copy.outerColor;
        this.innerColor = copy.innerColor;
        this.outerThreshold = copy.outerThreshold;
        this.innerThreshold = copy.innerThreshold;
    }

    public static GlowData Lerp(GlowData left, GlowData right, float lerpAmount)
    {
        GlowData copy = new GlowData(left);
        copy.innerColor = Color.Lerp(left.innerColor, right.innerColor, lerpAmount);
        copy.outerColor = Color.Lerp(left.outerColor, right.outerColor, lerpAmount);
        copy.outerThreshold = Mathf.Lerp(left.outerThreshold, right.outerThreshold, lerpAmount);
        copy.innerThreshold = Mathf.Lerp(left.innerThreshold, right.innerThreshold, lerpAmount);

        return copy;
    }
}
