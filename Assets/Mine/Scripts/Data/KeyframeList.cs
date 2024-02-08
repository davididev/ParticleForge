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

    [SerializeField]
    public List<KeyframeData<float>> FresnelKeyframes = new List<KeyframeData<float>>();

    [SerializeField]
    public List<KeyframeData<Vector3>> DirectionalLightRotationKeyframes = new List<KeyframeData<Vector3>>();
    [SerializeField]
    public List<KeyframeData<float>> DirectionalLightIntensityKeys = new List<KeyframeData<float>>();
    [SerializeField]
    public List<KeyframeData<float>> SceneLightIntensityKeys = new List<KeyframeData<float>>();

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

        var comparer6 = new FrameNumComparer<float>();
        FresnelKeyframes.Sort(comparer6);

        DirectionalLightRotationKeyframes.Sort(comparer1);  //Vector3 comparer
        DirectionalLightIntensityKeys.Sort(comparer6);  //Float comparer
        SceneLightIntensityKeys.Sort(comparer6);  //Float comparer


        KeyframeMainWindow window = KeyframeMainWindow.GetInstance();
        if(window != null)
        {
            window.UpdateKeyframes();
        }

    }
    /// <summary>
    /// Called whenever a rotation is changed
    /// </summary>
    /// <param name="frameNum">Frame number in timeline</param>
    /// <param name="rot">Object to set to</param>
    public void AddKeyframeRotation(int frameNum, Vector3 rot)
    {
        KeyframeData<Vector3>.AddOrUpdate(frameNum, rot, RotationKeyframes);

        SortAll();
    }

    /// <summary>
    /// Called whenever object mode position is updated
    /// </summary>
    /// <param name="frameNum">Frame number in the timeline</param>
    /// <param name="pos">Position to set to</param>
    public void AddKeyframePosition(int frameNum, Vector2 pos)
    {
        KeyframeData<Vector2>.AddOrUpdate(frameNum, pos, PositionKeyframes);
        SortAll();
    }

    public void AddKeyframeShape(int frameNum, Vector3[] points)
    {
        KeyframeData<ShapeData>.AddOrUpdate(frameNum, new ShapeData(points), ShapeKeyframes);

        SortAll();
    }

    /// <summary>
    /// Called whenever texture offset is updated
    /// </summary>
    /// <param name="frameNum">Frame number in the timeline</param>
    /// <param name="pos">Position to set to</param>
    public void AddKeyframeNoiseOffset(int frameNum, Vector2 pos)
    {
        KeyframeData<Vector2>.AddOrUpdate(frameNum, pos, NoiseTextureKeyframes);
        SortAll();
    }

    /// <summary>
    /// Called whenever color is updated
    /// </summary>
    /// <param name="frameNum">Frame number in the timeline</param>
    /// <param name="pos">Position to set to</param>
    public void AddKeyframeColor(int frameNum, Color c)
    {
        KeyframeData<Color>.AddOrUpdate(frameNum, c, ColorKeyframes);
        SortAll();
    }

    /// <summary>
    /// Called whenever a vertex is updated.  Should be called AFTER the mouse exits
    /// </summary>
    /// <param name="frameNum">Frame number in the timeline</param>
    /// <param name="pos">Position to set to</param>
    public void AddKeyVertexUpdated(int frameNum, ShapeData shape)
    {
        KeyframeData<ShapeData>.AddOrUpdate(frameNum, shape, ShapeKeyframes);
        SortAll();
    }


    /// <summary>
    /// Called whenever fresnel is updated.  Should be called AFTER the mouse exits
    /// </summary>
    /// <param name="frameNum">Frame number in the timeline</param>
    /// <param name="threshold">-1f to 1f, for fade</param>
    public void AddKeyFresnel(int frameNum, float threshold)
    {
        KeyframeData<float>.AddOrUpdate(frameNum, threshold, FresnelKeyframes);
        SortAll();
    }



    /// <summary>
    /// Create the default object, will add the vertices code later
    /// </summary>
    public void NewFile(int vertices = 500)
    {
        KeyframeData<Vector3> firstRot = new KeyframeData<Vector3>(1, new Vector3(-90f, 90f, -90f));
        RotationKeyframes.Add(firstRot);

        KeyframeData<Vector2> firstPos = new KeyframeData<Vector2>(1, Vector2.zero);
        PositionKeyframes.Add(firstPos);

        KeyframeData<Vector2> firstNoiseOffset = new KeyframeData<Vector2>(1, Vector2.zero);
        NoiseTextureKeyframes.Add(firstNoiseOffset);

        KeyframeData<Color> firstColor = new KeyframeData<Color>(1, Color.white);
        ColorKeyframes.Add(firstColor);

        KeyframeData<float> firstFresnold = new KeyframeData<float>(1, 1f);
        FresnelKeyframes.Add(firstFresnold);

        ShapeData.MAX_VERTICES = vertices;

        KeyframeData<ShapeData> firstShape = new KeyframeData<ShapeData>(1, new ShapeData());
        ShapeKeyframes.Add(firstShape);


        KeyframeData<Vector3> firstLightRot = new KeyframeData<Vector3>(1, new Vector3(0f, 180f, 0f));
        DirectionalLightRotationKeyframes.Add(firstLightRot);

        KeyframeData<float> firstLightIntensity = new KeyframeData<float>(1, 0f);
        DirectionalLightIntensityKeys.Add(firstLightIntensity);

        KeyframeData<float> firstLightIntensity2 = new KeyframeData<float>(1, 1f);
        SceneLightIntensityKeys.Add(firstLightIntensity2);
    }   


}
