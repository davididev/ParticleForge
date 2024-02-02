using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SkywardRay.FileBrowser;

public class NewFileWindow : MonoBehaviour
{
    public TMPro.TMP_InputField FrameSizeText;
    public Slider FrameCountSlider;  //Please note that the value should be squared for the actual value
    public TMPro.TextMeshProUGUI FrameCountSliderLabel;
    public TMPro.TMP_Dropdown DropDown;
    public SkywardFileBrowser fileBrowser;

    // Start is called before the first frame update
    void OnEnable()
    {
        DropDown.value = 0;
        FrameSizeText.text = "128.0";
        FrameCountSlider.value = 2;
        UpdateSlider();  //Draw the label
    }

    public void CreateNewFile()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        fileBrowser.SaveFile(path, OnFileCreated, new string[] { ".prt" });
    }

    public void UpdateSlider()
    {
        int frameCount = (int)FrameCountSlider.value;
        frameCount = frameCount * frameCount;

        FrameCountSliderLabel.text = frameCount.ToString();
    }

    public void OnFileCreated(string[] output)
    {
        int frameCount = (int)FrameCountSlider.value;
        frameCount = frameCount * frameCount;
        PartFile.GetInstance().NewFile(output[0], float.Parse(FrameSizeText.text), frameCount, DropDown.value);

        SceneManager.LoadScene("ParticleEditor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
