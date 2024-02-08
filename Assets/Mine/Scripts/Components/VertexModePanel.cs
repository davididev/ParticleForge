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
            toolDescritption.text = "Left click and drag to select point.\nRight click to clear selection.\nYou need at 1+ point to use move, 2+ points to use scale/rotate";
            vertexSelector.gameObject.SetActive(true);
        }
        if (m == 1)
        {
            if (VertexUI.Selected.Count < 1)  //Nothing selected yet- can't edit
            {
                SetMode(0);
                return;
            }
            LastNewWorldPosition = Vector3.zero;  //Reset tool processing variable
            toolHeader.text = "Move Vertex";
            toolDescritption.text = "(after making selection) \nMove all selected points on the X / Y Axis";
            MoveTransformTools.SetActive(true);
            MoveTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
        if (m == 2)
        {
            if (VertexUI.Selected.Count < 2)  //Nothing selected yet- can't edit
            {
                SetMode(0);
                return;
            }
            toolHeader.text = "Scale Vertex";
            toolDescritption.text = "(after making selection) \nScale all selected points on the X / Y Axis from the midpoint of all points.";
            ScaleTransformTools.SetActive(true);
            ScaleTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
        if (m == 3)
        {
            if (VertexUI.Selected.Count < 2)  //Nothing selected yet- can't edit
            {
                SetMode(0);
                return;
            }
            toolHeader.text = "Rotate Vertex";
            toolDescritption.text = "(after making selection) \nRotate all selected points on the Z Axis from the midpoint of all points.";
            RotateTransformTools.SetActive(true);
            RotateTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MyMode == 1)
            PositionUpdate();
        if (MyMode == 2)
            ScaleUpdate();
        if (MyMode == 3)
            RotationUpdate();
    }

    Vector3 LastNewWorldPosition = Vector3.zero;
    void PositionUpdate()
    {
        
        if (TransformArrow.IsDragging == false)  //Don't run this unless you're actually moving the arrows
            return;
        if (LastNewWorldPosition == Vector3.zero)
        {
            LastNewWorldPosition = TransformArrow.NewWorldPosition;
            return;
        }

        Vector3 rel = TransformArrow.NewWorldPosition - LastNewWorldPosition;

        Vector3[] tempMesh = refToShape.GetVertices();
        List<VertexUI>.Enumerator e1 = VertexUI.Selected.GetEnumerator();  //Each vertex UI stores a mesh vertex ID in it
        while(e1.MoveNext())  //Move each Selected vertex by rel
        {
            tempMesh[e1.Current.VertexID] += new Vector3(rel.x / 100f, 0f, rel.y / 100f); 
        }

        refToShape.SetVertices(tempMesh);
        LastNewWorldPosition = TransformArrow.NewWorldPosition;
    }

    void ScaleUpdate()
    {

    }

    void RotationUpdate()
    {

    }

    public void AddKeyframe()
    {
        PartFile.GetInstance().KeyFrames.AddKeyframeShape(KeyframeMainWindow.SelectedFrame, refToShape.GetVertices());
    }
}
