using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleRootUI : MonoBehaviour
{
    public GameObject[] Panels;
    // Start is called before the first frame update
    void Start()
    {
        SetPanel(0);
    }

    public void QuitProgram()
    {
        Application.Quit();
    }

    public void SetPanel(int id)
    {
        for(int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(id == i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
