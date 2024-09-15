using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertexModePanel : MonoBehaviour
{
    private int MyMode = 0;
    public StartingShapeHolder refToShape;
    public VertexSelector vertexSelector;

    public TMPro.TextMeshProUGUI toolHeader, toolDescritption;
    public TMPro.TMP_InputField snapTextBox;

    public GameObject MoveTransformTools, ScaleTransformTools, RotateTransformTools, snapHolder;
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
            snapHolder.SetActive(false);
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
            VertexUI.CalculateMidpoint();
            snapHolder.SetActive(false);
            LastNewWorldPosition = Vector3.zero;  //Reset tool processing variable
            toolHeader.text = "Move Vertex";
            toolDescritption.text = "(after making selection) \nMove all selected points on the X / Y Axis";
            MoveTransformTools.SetActive(true);
            MoveTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
            //MoveTransformTools.transform.position = VertexUI.Midpoint;
        }
        if (m == 2)  //Scale mode
        {
            if (VertexUI.Selected.Count < 2)  //Nothing selected yet- can't edit
            {
                SetMode(0);
                return;
            }
            snapHolder.SetActive(true);
            snapTextBox.text = TransformScaleArrow.ScaleSnap.ToString();
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
            snapHolder.SetActive(true);
            snapTextBox.text = TransformRotate.RotateSnap.ToString();
            rotateAngle = 0f;  //Tool temporary variable
            startingScaleVertexes = refToShape.GetVertices();
            localMidPoint = refToShape.CurrentShape.transform.InverseTransformPoint(VertexUI.Midpoint);

            toolHeader.text = "Rotate Vertex";
            toolDescritption.text = "(after making selection) \nRotate all selected points on the Z Axis from the midpoint of all points.";
            RotateTransformTools.SetActive(true);
            RotateTransformTools.transform.position = Camera.main.WorldToScreenPoint(VertexUI.Midpoint);
        }
    }

    public void OnUpdateSnapText()
    {
        if(MyMode == 2) // Scale
            TransformScaleArrow.ScaleSnap = float.Parse(snapTextBox.text);
        if (MyMode == 3) // Rotation
            TransformRotate.RotateSnap = float.Parse(snapTextBox.text);
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
            localMidPoint = refToShape.CurrentShape.transform.InverseTransformPoint(VertexUI.Midpoint);
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

    Vector3 localMidPoint;
    // Function to rotate a point around the Y-axis based on its current position, an origin point, and an angle
    public Vector3 RotatePoint(Vector3 point, Vector3 origin, float angle)
    {
        // Calculate the vector from the origin to the point
        Vector3 vectorToPoint = point - origin;

        // Calculate the sine and cosine of the angle
        float cosTheta = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sinTheta = Mathf.Sin(angle * Mathf.Deg2Rad);

        // Rotate the vector around the Y-axis
        float newX = vectorToPoint.x * cosTheta + vectorToPoint.z * sinTheta;
        float newZ = -vectorToPoint.x * sinTheta + vectorToPoint.z * cosTheta;

        // Create the rotated vector
        Vector3 rotatedVector = new Vector3(newX, vectorToPoint.y, newZ);

        // Add the rotated vector to the origin to get the rotated point
        Vector3 rotatedPoint = origin + rotatedVector;

        return rotatedPoint;
    }

    float rotateAngle = 0f;
    void RotationUpdate()
    {
        if(TransformRotate.OffsetStep != Vector3.zero)
        {
            rotateAngle += TransformRotate.OffsetStep.z;
            TransformRotate.OffsetStep = Vector3.zero;

        }


        

        //Debug.Log("Rotate arrow is now " + rotateAngle + "; dragging: " + TransformRotate.IsDragging.ToString());

        if (TransformRotate.IsDragging)
        {
            Vector3[] tempMesh = refToShape.GetVertices();
            List<VertexUI>.Enumerator e1 = VertexUI.Selected.GetEnumerator();  //Each vertex UI stores a mesh vertex ID in it
            //Debug.Log(Time.time + "-New Scale: " + TransformScaleArrow.NewScale.ToString());
            //localMidPoint = refToShape.CurrentShape.transform.InverseTransformPoint(VertexUI.Midpoint);
            while (e1.MoveNext())  //Move each Selected vertex by rel
            {
                int thisVert = e1.Current.VertexID;

                Vector3 temp = RotatePoint(startingScaleVertexes[thisVert], localMidPoint, rotateAngle);
                tempMesh[thisVert] = temp;

            }
            refToShape.SetVertices(tempMesh);
        }
    }

    public void AddKeyframe()
    {
        PartFile.GetInstance().KeyFrames.AddKeyframeShape(KeyframeMainWindow.SelectedFrame, refToShape.GetVertices());
    }
}
