using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingShapeHolder : MonoBehaviour
{
    public GameObject[] StartingShapes;
    private Vector3[] StartingVertices;

    public GameObject StartingVertState;
    private GameObject[] VerticesObject;
    public static List<VertexUI> VertexUIList = new List<VertexUI>();

    public GameObject CurrentShape { private set; get; }
    // Start is called before the first frame update
    void Start()
    {
        int id = PartFile.GetInstance().ShapeID;
        //Only show the selected shape
        for (int i = 0; i < StartingShapes.Length; i++)
        {
            StartingShapes[i].SetActive(id == i);
        }

        StartingVertices = StartingShapes[id].GetComponent<MeshFilter>().mesh.vertices;
        CurrentShape = StartingShapes[id];
        VerticesObject = new GameObject[StartingVertices.Length];
        VerticesObject[0] = StartingVertState;
        VerticesObject[0].transform.parent = CurrentShape.transform;
        VerticesObject[0].transform.localPosition = StartingVertices[0];
        VertexUIList.Add(VerticesObject[0].GetComponent<VertexUI>());
        for (int i = 1; i < VerticesObject.Length; i++)
        {
            VerticesObject[i] = GameObject.Instantiate(StartingVertState, StartingVertState.transform.parent, false) as GameObject;
            VerticesObject[i].GetComponent<VertexUI>().VertexID = i;
            VerticesObject[i].transform.localPosition = StartingVertices[i];
            VertexUIList.Add(VerticesObject[i].GetComponent<VertexUI>());
        }

        PartFile.GetInstance().KeyFrames.AddKeyVertexUpdated(1, new ShapeData(StartingVertices));
        //ShowOrHideVertices(false);
    }

    /// <summary>
    /// Should be called on startup and when you enable/disable vertex panel
    /// </summary>
    /// <param name="visible"></param>
    public void ShowOrHideVertices(bool visible)
    {
        for(int i = 0; i < VerticesObject.Length; i++)
        {
            VerticesObject[i].SetActive(visible);
            if(visible)
            {
                VertexUI.ClearHovered();
                VertexUI.ClearSelected();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
