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
        MoveTransformTools.SetActive(false);
        ScaleTransformTools.SetActive(false);
        RotateTransformTools.SetActive(false);
    }

    public void SetMode(int m)
    {

        MyMode = m;
        MoveTransformTools.SetActive(false);
        ScaleTransformTools.SetActive(false);
        RotateTransformTools.SetActive(false);
        vertexSelector.gameObject.SetActive(false);
        
        if (m == 0)  //Select mode
        {
            toolHeader.text = "Vertex Select ";
            toolDescritption.text = "Left click and drag to select point.\nRight click to clear selection.\nYou need at 1+ point to use move, 2+ points to use scale/rotate";
            vertexSelector.gameObject.SetActive(true);
        }
        if (m == 1)  //Translation mode
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
        if (m == 2)  //Scale mode
        {
            if (VertexUI.Selected.Count < 2)  //Nothing selected yet- can't edit
            {
                SetMode(0);
                return;
            }
            //Starting vars
            startingScaleVertexes = refToShape.GetVertices();
            TransformScaleArrow.NewScale = Vector3.one;

            toolHeader.text = "Scale Vertex";
            toolDescritption.text = "Scale all selected points on the X / Y Axis from the midpoint of all points.";
            ScaleTransformTools.SetActive(true);
            ScaleTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
        if (m == 3)  //Rotate mode
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

    Vector3[] startingScaleVertexes;
    void ScaleUpdate()
    {
        
        if(TransformScaleArrow.IsDragging)
        {
            Vector3[] tempMesh = refToShape.GetVertices();
            List<VertexUI>.Enumerator e1 = VertexUI.Selected.GetEnumerator();  //Each vertex UI stores a mesh vertex ID in it
            //Debug.Log(Time.time + "-New Scale: " + TransformScaleArrow.NewScale.ToString());
            Vector3 localMidPoint = refToShape.CurrentShape.transform.InverseTransformPoint(VertexUI.Midpoint);
            Vector3 scale = new Vector3(TransformScaleArrow.NewScale.x, 0f, TransformScaleArrow.NewScale.y);
            while (e1.MoveNext())  //Move each Selected vertex by rel
            {
                int thisVert = e1.Current.VertexID;

                Vector3 temp = ScalePoint(startingScaleVertexes[thisVert], localMidPoint, scale);
                tempMesh[thisVert] = temp;

            }
            refToShape.SetVertices(tempMesh);
        }
        
    }

    public Vector3 ScalePoint(Vector3 point, Vector3 origin, Vector3 scale)
    {
        // Calculate the vector from the origin to the point
        Vector3 vectorToPoint = point - origin;

        // Scale the vector
        Vector3 scaledVector = new Vector3(
            vectorToPoint.x * scale.x,
            vectorToPoint.y * scale.y,
            vectorToPoint.z * scale.z
        );

        // Add the scaled vector to the origin to get the scaled point
        Vector3 scaledPoint = origin + scaledVector;

        return scaledPoint;
    }

    void RotationUpdate()
    {

    }

    public void AddKeyframe()
    {
        PartFile.GetInstance().KeyFrames.AddKeyframeShape(KeyframeMainWindow.SelectedFrame, refToShape.GetVertices());
    }
}
