using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkywardRay.FileBrowser;

public class ShapeOutlineExporter : MonoBehaviour
{
    public SkywardFileBrowser fileBrowser;
    
    public void ExportOutlineButton()
    {
        string dir = DirectoryHelper.GetDirectoryOfFile(PlayerPrefs.GetString("LastFile"));
        fileBrowser.SaveFile(dir, NoiseSaved, new string[] { ".png" });
    }

    void NoiseSaved(string[] output)
    {
        string path = output[0];
        string fileType = path.Substring(path.Length - 4);
        if (fileType != ".png")
        {
            path += ".png";
        }
        Texture2D original = Resources.Load<Texture2D>("Mask/Preview_" + PartFile.GetInstance().ShapeID);
        Texture2D copyOfOriginal = new Texture2D(original.width, original.height);
        for(int x = 0; x < original.width; x++)
        {
            for(int y = 0; y < original.height; y++)
            {
                copyOfOriginal.SetPixel(x, y, original.GetPixel(x, y));
            }
        }
        copyOfOriginal.Apply();
        System.IO.File.WriteAllBytes(path, copyOfOriginal.EncodeToPNG());
        gameObject.SetActive(false);
    }
}
