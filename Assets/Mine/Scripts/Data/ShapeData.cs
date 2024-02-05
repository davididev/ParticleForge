using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeData
{
    public static int MAX_VERTICES = 150;
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
}
