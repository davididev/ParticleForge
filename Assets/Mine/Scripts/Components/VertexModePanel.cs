using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexModePanel : MonoBehaviour
{
    private int MyMode = 0;
    public StartingShapeHolder refToShape;
    public VertexSelector vertexSelector;
    public TMPro.TextMeshProUGUI toolHeader, toolDescritption;

    public GameObject MoveTransformTools, ScaleTransformTools, RotateTransformTools;
    // Start is called before the first frame update
    void OnEnable()
    {
        SetMode(0);
        vertexSelector.gameObject.SetActive(true);
        refToShape.ShowOrHideVertices(true);
    }

    private void OnDisable()
    {
        vertexSelector.gameObject.SetActive(false);
        refToShape.ShowOrHideVertices(false);
    }

    public void SetMode(int m)
    {
        MyMode = m;
        MoveTransformTools.SetActive(false);
        ScaleTransformTools.SetActive(false);
        RotateTransformTools.SetActive(false);
        vertexSelector.gameObject.SetActive(false);
        if (m == 0)
        {
            toolHeader.text = "Vertex Select ";
            toolDescritption.text = "Left click and drag to select point.\nRight click to clear selection";
            vertexSelector.gameObject.SetActive(true);
        }
        if (m == 1)
        {
            toolHeader.text = "Move Vertex";
            toolDescritption.text = "(after making selection) \nMove all selected points on the X / Y Axis";
            MoveTransformTools.SetActive(true);
            MoveTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
        if (m == 2)
        {
            toolHeader.text = "Scale Vertex";
            toolDescritption.text = "(after making selection) \nScale all selected points on the X / Y Axis from the midpoint of all points.";
            ScaleTransformTools.SetActive(true);
            ScaleTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
        if (m == 3)
        {
            toolHeader.text = "Rotate Vertex";
            toolDescritption.text = "(after making selection) \nRotate all selected points on the Z Axis from the midpoint of all points.";
            RotateTransformTools.SetActive(true);
            RotateTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddKeyframe()
    {

    }
}
