using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyframeList 
{
    [SerializeField]
    public List<KeyframeData<Vector3>> RotationKeyframes = new List<KeyframeData<Vector3>>();
    [SerializeField]
    public List<KeyframeData<Vector2>> PositionKeyframes = new List<KeyframeData<Vector2>>();
    [SerializeField]
    public List<KeyframeData<Vector2>> NoiseTextureKeyframes = new List<KeyframeData<Vector2>>();
    [SerializeField]
    public List<KeyframeData<Color>> ColorKeyframes = new List<KeyframeData<Color>>();
    [SerializeField]
    public List<KeyframeData<ShapeData>> ShapeKeyframes = new List<KeyframeData<ShapeData>>();

    /// <summary>
    /// Called every time we add, delete, or move a keyframe
    /// </summary>
    public void SortAll()
    {
        var comparer1 = new FrameNumComparer<Vector3>();
        RotationKeyframes.Sort(comparer1);

        var comparer2 = new FrameNumComparer<Vector2>();
        PositionKeyframes.Sort(comparer2);

        var comparer3 = new FrameNumComparer<Vector2>();
        NoiseTextureKeyframes.Sort(comparer3);

        var comparer4 = new FrameNumComparer<Color>();
        ColorKeyframes.Sort(comparer4);

        var comparer5 = new FrameNumComparer<ShapeData>();
        ShapeKeyframes.Sort(comparer5);
    }

    /// <summary>
    /// Create the default object, will add the vertices code later
    /// </summary>
    public void NewFile(int vertices = 500)
    {
        KeyframeData<Vector3> firstRot = new KeyframeData<Vector3>(1, new Vector3(-90f, 0f, 0f));
        RotationKeyframes.Add(firstRot);

        KeyframeData<Vector2> firstPos = new KeyframeData<Vector2>(1, Vector2.zero);
        PositionKeyframes.Add(firstPos);

        KeyframeData<Vector2> firstNoiseOffset = new KeyframeData<Vector2>(1, Vector2.zero);
        NoiseTextureKeyframes.Add(firstNoiseOffset);

        KeyframeData<Color> firstColor = new KeyframeData<Color>(1, Color.white);
        ColorKeyframes.Add(firstColor);

        ShapeData.MAX_VERTICES = vertices;

        KeyframeData<ShapeData> firstShape = new KeyframeData<ShapeData>(1, new ShapeData());
        ShapeKeyframes.Add(firstShape);
    }


}
