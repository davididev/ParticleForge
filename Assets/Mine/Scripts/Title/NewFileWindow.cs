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

    public GameObject[] startingShapePages;
    public TMPro.TMP_Dropdown DropDown;
    public SkywardFileBrowser fileBrowser;
    int currentStartingPage = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        currentStartingPage = 0;
        StartingShapeButton.CurrentSelectedShape = 0;
        //DropDown.value = 0;
        SetPageID(0);
        FrameSizeText.text = "128";
        FrameCountSlider.value = 2;
        UpdateSlider();  //Draw the label
    }

    public void OnUpdateFrameSizeText()
    {
        int size = int.Parse(FrameSizeText.text);
        size = Mathf.Clamp(size, 32, 512);
        FrameSizeText.text = size.ToString("0.0");
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
        //PartFile.GetInstance().NewFile(output[0], int.Parse(FrameSizeText.text), frameCount, DropDown.value);
        PartFile.GetInstance().NewFile(output[0], int.Parse(FrameSizeText.text), frameCount, StartingShapeButton.CurrentSelectedShape);

        SceneManager.LoadScene("ParticleEditor");
    }

    public void OnNextPagePressed()
    {
        SetPageID(currentStartingPage + 1);
    }

    public void OnPrevPagePressed()
    {
        SetPageID(currentStartingPage - 1);
    }

    public void SetPageID(int id)
    {
        if (id < 0)
            id = startingShapePages.Length - 1;
        if (id >= startingShapePages.Length)
            id = 0;
        currentStartingPage = id;

        for (int i = 0; i < startingShapePages.Length; i++)
        {
            startingShapePages[i].SetActive(i == id);
            if(id == i)
            {
                startingShapePages[i].transform.GetChild(0).gameObject.SendMessage("HighlightButton");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
