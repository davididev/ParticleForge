using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNoiseOffsetPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public StartingShapeHolder refToShape;
    public TMPro.TMP_InputField xInputField, yInputField;
    void Start()
    {
        
    }

    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        Vector2 offset = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetTextureOffset("_Noise");
        xInputField.text = offset.x.ToString("0.0");
        yInputField.text = offset.y.ToString("0.0");
    }

    public void OnUpdateTextFields()  //When you update the text boxes
    {
        float x = float.Parse(xInputField.text);
        float y = float.Parse(yInputField.text);
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetTextureOffset("_Noise", new Vector2(x, y));
    }

    public void AddKeyframe()
    {
        float x = float.Parse(xInputField.text);
        float y = float.Parse(yInputField.text);
        PartFile.GetInstance().KeyFrames.AddKeyframeNoiseOffset(KeyframeMainWindow.SelectedFrame, new Vector2(x, y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
