using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkywardRay.FileBrowser;
using UnityEngine.SceneManagement;


public class StartupWindow : MonoBehaviour
{
    public SkywardFileBrowser fileBrowser;
    // Start is called before the first frame update
    void OnEnable()
    {
        
    }

    public void LoadFileButton()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        fileBrowser.OpenFile(path, OnFileLoaded, new string[] { ".prt" });
    }

    public void OnFileLoaded(string[] output)
    {
        try
        {
            PartFile.GetInstance().LoadFile(output[0]);
        }
        catch(System.IO.FileNotFoundException e)
        {
            Debug.LogError("Could not load file.  " + e.ToString());
            return;
        }

        SceneManager.LoadScene("ParticleEditor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
