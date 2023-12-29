using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartFile
{
    public string FileDirectory { get; private set; }
    public string NoiseDirectory = "";
    public Color StartingColor = Color.white;
    public float FrameSize = 32f;
    public int FrameCount = 4;
    public KeyframeList KeyFrames;

    private static PartFile Instance;

    public void NewFile(string FileName)
    {
        Instance = new PartFile();
        SaveFile(FileName);
    }

    public void SaveFile(string FileName)
    {
        int lastIndex = FileName.LastIndexOf('\\');
        FileDirectory = FileName.Substring(0, lastIndex);

        string fileExtension = FileName.Substring(FileName.Length - 5, 5);
        if (fileExtension != ".part")
            FileName += ".part";


        string contents = JsonUtility.ToJson(Instance);
        System.IO.File.WriteAllText(FileName, contents);
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
