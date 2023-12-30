using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingShapeHolder : MonoBehaviour
{
    public GameObject[] StartingShapes;
    private Vector3[] StartingVertices;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
