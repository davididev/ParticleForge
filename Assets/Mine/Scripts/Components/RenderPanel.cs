using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderPanel : MonoBehaviour
{
    public GameObject mainCanvas;
    public OutputCamera outputCamera;
    public RawImage previewImage;

    public TMPro.TMP_InputField cameraSizeText, frameSizeText;
    public TMPro.TextMeshProUGUI outputSizeLabel;

    // Start is called before the first frame update
    void OnEnable()
    {
        frameSizeText.text = PartFile.GetInstance().FrameSize.ToString();
        cameraSizeText.text = "5.00";
        OnUpdateFrameSizeText();
        OnUpdateCameraSizeText();
        mainCanvas.SetActive(false);
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
        int sizeX = squareFrames * size;

        outputSizeLabel.text = "Output Size: " + sizeX + "x" + sizeX;
    }

    private void OnDisable()
    {
        mainCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
