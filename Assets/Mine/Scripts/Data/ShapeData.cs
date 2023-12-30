using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeData
{
    public static int MAX_VERTICES = 150;
    [SerializeField]
    public List<Vector2> Vertices = new List<Vector2>();  //These are offsets, not world positions

    /// <summary>
    /// Create default (empty)
    /// </summary>
    public ShapeData()
    {
        for(int i = 0; i < MAX_VERTICES; i++)
        {
            Vertices.Add(Vector2.zero);
        }
    }

    /// <summary>
    /// Create from Mesh Editor
    /// </summary>
    /// <param name="verts">Offset from world cooridnates</param>
    public ShapeData(Vector2[] verts)
    {
        Vertices.Clear();
        for(int i = 0; i < verts.Length; i++)
        {
            Vertices.Add(verts[i]);
        }
    }
}
