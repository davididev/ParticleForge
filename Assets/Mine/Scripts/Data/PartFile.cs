using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int lastIndex = FileName.LastIndexOf('\\');
        if(lastIndex <= 0)
            lastIndex = FileName.LastIndexOf('/');
        FileDirectory = FileName.Substring(0, lastIndex);

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
            return;
        }


        Instance = JsonUtility.FromJson<PartFile>(System.IO.File.ReadAllText(FileName));
        int lastIndex = FileName.LastIndexOf('\\');
        if (lastIndex <= 0)
            lastIndex = FileName.LastIndexOf('/');
        FileDirectory = FileName.Substring(0, lastIndex);

    }
    public Texture2D LoadNoise(string NoiseDirectory)
    {
        //Will write code later
        return null;
    }

    public static PartFile GetInstance()
    {
        if (Instance == null)
            Instance = new PartFile();

        return Instance;
    }
    
}
