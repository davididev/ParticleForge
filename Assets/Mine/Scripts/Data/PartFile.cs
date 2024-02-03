using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PartFile
{
    public string FileDirectory { get; private set; }
    public string NoiseDirectory = "";
    public float FrameSize = 32f;
    public int FrameCount = 16;
    public int ShapeID = 0;
    public KeyframeList KeyFrames;

    private static PartFile Instance;

    public PartFile()
    {
        KeyFrames = new KeyframeList();
        KeyFrames.NewFile();
    }

    public void NewFile(string FileName, float frameSize, int frameCount, int shapeID)
    {
        Instance = new PartFile();
        Instance.FrameSize = frameSize;
        Instance.FrameCount = frameCount;
        Instance.ShapeID = shapeID;
        KeyFrames.NewFile();
        SaveFile(FileName);
    }

    public void SaveFile(string FileName)
    {
        FileDirectory = DirectoryHelper.GetDirectoryOfFile(FileName);

        string fileExtension = FileName.Substring(FileName.Length - 4, 4);
        if (fileExtension != ".prt")
            FileName += ".prt";


        string contents = JsonUtility.ToJson(Instance);
        System.IO.File.WriteAllText(FileName, contents);

        PlayerPrefs.SetString("LastFile", FileName);  //For editor purposes
    }

    public void LoadFile(string FileName)
    {
        string fileExtension = FileName.Substring(FileName.Length - 4, 4);
        Debug.Log("File Extension: " + fileExtension);
        if (fileExtension != ".prt")
        {
            throw new System.IO.FileNotFoundException();
        }


        Instance = JsonUtility.FromJson<PartFile>(System.IO.File.ReadAllText(FileName));
        FileDirectory = DirectoryHelper.GetDirectoryOfFile(FileName);
        PlayerPrefs.SetString("LastFile", FileName);  //For editor purposes
    }


    public Texture2D LoadNoise()
    {
        //Will write code later
        Texture2D newTex = new Texture2D((int)FrameSize, (int)FrameSize);
        string dir = PlayerPrefs.GetString("LastFile");
        dir = DirectoryHelper.GetDirectoryOfFile(dir);
        Debug.Log("File directory: " + dir + "; " + NoiseDirectory);

        if (NoiseDirectory != "")
        {
            string newDir = DirectoryHelper.CombineDirectories(dir, NoiseDirectory);
            Debug.Log("New Dir: " + newDir);
            if (File.Exists(newDir))
            {
                byte[] data = System.IO.File.ReadAllBytes(newDir);  //Relative to the file we're in
                if (newTex.LoadImage(data))
                {
                    newTex.Apply();
                    return newTex;
                }
            }   
        }
            
        
        

        //Fail code here
        Texture2D white = new Texture2D(32, 32);
        for(int x = 0; x < white.width; x++)
        {
            for (int y = 0; y < white.height; y++)
            {
                white.SetPixel(x, y, Color.white);
            }
        }
        white.Apply();
        return white;
    }

    public static PartFile GetInstance()
    {
        if (Instance == null)
            Instance = new PartFile();

        return Instance;
    }
    
}
