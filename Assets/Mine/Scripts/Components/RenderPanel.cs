using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkywardRay.FileBrowser;

public class RenderPanel : MonoBehaviour
{
    public GameObject toolsRoot;
    public KeyframeMainWindow mainWindow;
    public OutputCamera outputCamera;
    public RawImage previewImage;
    public SkywardFileBrowser fileBrowser;

    public GameObject settingsRoot, renderingRoot;

    public TMPro.TMP_InputField cameraSizeText, frameSizeText;
    public Toggle ToggleButton;
    public Toggle MultipleFileButtons, LeadingZerosButton;
    public GameObject MultipleFileHolder;
    public TMPro.TextMeshProUGUI outputSizeLabel, renderingLabel;

    private int spritesheetSize = 0;  //Size of the whole spritesheet


    // Start is called before the first frame update
    void OnEnable()
    {
        settingsRoot.SetActive(true);
        renderingRoot.SetActive(false);
        mainWindow.CurrentMode = KeyframeMainWindow.OBJECT_MODE.Render;
        frameSizeText.text = PartFile.GetInstance().FrameSize.ToString();
        cameraSizeText.text = "1.50";
        OnUpdateFrameSizeText();
        OnUpdateCameraSizeText();
        OnPressToggleMultipleFiles();
        toolsRoot.SetActive(false);
    }

    public void OnClickRenderSpritesheet()
    {
        string dir = DirectoryHelper.GetDirectoryOfFile(PlayerPrefs.GetString("LastFile"));
        fileBrowser.SaveFile(dir, OnFileCreated, new string[] { ".png"});
    }

    public void OnPressToggleMultipleFiles()
    {
        MultipleFileHolder.SetActive(MultipleFileButtons.isOn);
    }

    void OnFileCreated(string[] output)
    {
        if(MultipleFileButtons.isOn)  //Adding this so we can split them up
        {
            //New added code
            StartCoroutine(RenderingRoutineSeperate(output[0]));
        }
        else  //Original code for sprite sheet
        {
            string fileExtension = output[0].Substring(output[0].Length - 4, 4);
            if (fileExtension != ".png")
                output[0] += ".png";
            StartCoroutine(RenderingRoutine(output[0]));
        }
        
    }

    IEnumerator RenderingRoutineSeperate(string path)
    {
        renderingRoot.SetActive(true);
        settingsRoot.SetActive(false);
        int maxFrame = PartFile.GetInstance().FrameCount;

        Texture2D[] frames = new Texture2D[maxFrame];
        int frameSize = PartFile.GetInstance().FrameSize;

        //Create indiv frames as textures
        for (int i = 1; i <= maxFrame; i++)
        {
            KeyframeMainWindow.SelectedFrame = i;
            mainWindow.RefreshObjectState();
            yield return new WaitForSeconds(0.05f);
            renderingLabel.text = "Rendering frame " + i + " of " + maxFrame;
            int z = i - 1;  //Array index

            frames[z] = new Texture2D(frameSize, frameSize);

            Texture2D CurFrame = outputCamera.ConvertedTex();
            for (int x = 0; x < frameSize; x++)
            {
                for (int y = 0; y < frameSize; y++)
                {
                    Color c = CurFrame.GetPixel(x, y);
                    frames[z].SetPixel(x, y, c);
                }
            }
            if (ToggleButton.isOn)  //Convert black to alpha
                frames[z] = outputCamera.TextureToAlpha(frames[z]);
            frames[z].Apply();
            yield return new WaitForSeconds(0.1f);
        }

        //Combine the frames into a final image
        renderingLabel.text = "Done";
        

        for(int i = 1; i <= maxFrame; i++)
        {
            int z = i - 1;  //Array index
            string newPath = path;
            if (LeadingZerosButton.isOn)
                newPath += i.ToString("D2") + ".png";
            else
                newPath += i + ".png";
            System.IO.File.WriteAllBytes(newPath, frames[z].EncodeToPNG());
            renderingLabel.text = "Writing frame " + i + " of " + maxFrame;
            yield return new WaitForSeconds(0.05f);
        }
        

        renderingLabel.text = "Done!";
        yield return new WaitForSeconds(0.5f);
        renderingRoot.SetActive(false);
        settingsRoot.SetActive(true);
    }

    IEnumerator RenderingRoutine(string path)
    {
        renderingRoot.SetActive(true);
        settingsRoot.SetActive(false);
        int maxFrame = PartFile.GetInstance().FrameCount;

        Texture2D[] frames = new Texture2D[maxFrame];
        int frameSize = PartFile.GetInstance().FrameSize;

        //Create indiv frames as textures
        for (int i = 1; i <= maxFrame; i++)
        {
            KeyframeMainWindow.SelectedFrame = i;
            mainWindow.RefreshObjectState();
            yield return new WaitForSeconds(0.05f);
            renderingLabel.text = "Rendering frame " + i + " of " + maxFrame;
            int z = i - 1;  //Array index
            
            frames[z] = new Texture2D(frameSize, frameSize);

            Texture2D CurFrame = outputCamera.ConvertedTex();
            for(int x = 0; x < frameSize; x++)
            {
                for(int y = 0; y < frameSize; y++)
                {
                    Color c = CurFrame.GetPixel(x, y);
                    frames[z].SetPixel(x, y, c);
                }
            }

            frames[z].Apply();
            
        }

        //Combine the frames into a final image
        renderingLabel.text = "Rendering final image";
        Texture2D finalTex = new Texture2D(spritesheetSize, spritesheetSize);
        int sqrt = Mathf.RoundToInt(Mathf.Sqrt((float)maxFrame));
        int id = 0;
        for (int y1 = sqrt-1; y1 >= 0; y1--) 
        {
            for (int x1 = 0; x1 < sqrt; x1++)
            {
                for (int x = 0; x < frameSize; x++)
                {
                    for (int y = 0; y < frameSize; y++)
                    {
                        int xCoord = (x1 * frameSize) + x;  //Coords of final texture
                        int yCoord = (y1 * frameSize) + y;  //Coords of final texture
                        Color c = frames[id].GetPixel(x, y);  //Color of current frame
                        finalTex.SetPixel(xCoord, yCoord, c);
                        
                    }
                }
                id++;
            }    
        }

        if (ToggleButton.isOn)  //Convert black to alpha
            finalTex = outputCamera.TextureToAlpha(finalTex);

        finalTex.Apply();
        System.IO.File.WriteAllBytes(path, finalTex.EncodeToPNG());

        renderingLabel.text = "Done!";
        yield return new WaitForSeconds(0.5f);
        renderingRoot.SetActive(false);
        settingsRoot.SetActive(true);
    }

    public void OnUpdateCameraSizeText()
    {
        float size = float.Parse(cameraSizeText.text);
        size = Mathf.Clamp(size, 1f, 5f);
        cameraSizeText.text = size.ToString("0.00");
        outputCamera.SetSize(size);
    }

    public void OnUpdateFrameSizeText()
    {
        int size = int.Parse(frameSizeText.text);
        size = Mathf.Clamp(size, 32, 512);
        PartFile.GetInstance().FrameSize = size;

        int squareFrames = Mathf.RoundToInt(Mathf.Sqrt((float)PartFile.GetInstance().FrameCount));
        spritesheetSize = squareFrames * size;

        outputSizeLabel.text = "Output Size: " + spritesheetSize + "x" + spritesheetSize;

        outputCamera.RedefineRenderTexture(size);
        previewImage.texture = outputCamera.currentTexture;
    }

    private void OnDisable()
    {
        toolsRoot.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
