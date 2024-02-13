using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeData
{
    public static int MAX_VERTICES = 400;
    [SerializeField]
    public List<Vector3> Vertices = new List<Vector3>();  //These are offsets, not world positions

    /// <summary>
    /// Create default (empty)
    /// </summary>
    public ShapeData()
    {
        for(int i = 0; i < MAX_VERTICES; i++)
        {
            Vertices.Add(Vector3.zero);
        }
    }

    /// <summary>
    /// Create from Mesh Editor
    /// </summary>
    /// <param name="verts">Offset from world cooridnates</param>
    public ShapeData(Vector3[] verts)
    {
        Vertices.Clear();
        for(int i = 0; i < verts.Length; i++)
        {
            Vertices.Add(verts[i]);
        }
    }

    public ShapeData(List<Vector3> verts)
    {
        Vertices.Clear();
        List<Vector3>.Enumerator e1 = verts.GetEnumerator();
        while(e1.MoveNext())
        {
            Vertices.Add(e1.Current);
        }
    }

    public static ShapeData Lerp(ShapeData left, ShapeData right, float lerpAmt)
    {
        ShapeData tempLerp = new ShapeData(left.Vertices);
        int max = tempLerp.Vertices.Count;
        for(int i = 0; i < max; i++)
        {
            tempLerp.Vertices[i] = Vector3.Lerp(left.Vertices[i], right.Vertices[i], lerpAmt);
        }
        return tempLerp;
    }
}
