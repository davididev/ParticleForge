using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FresnelPanel : MonoBehaviour
{
    public StartingShapeHolder refToShape;
    public TMPro.TextMeshProUGUI sliderText;
    public Slider slider;
    // Start is called before the first frame update
    void OnEnable()
    {
        
    }

    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        float lerp = 0f;
        List<KeyframeData<float>> tempData2 = PartFile.GetInstance().KeyFrames.FresnelKeyframes;
        float fres1 = 0f;
        float fres2 = 0f;

        KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData2);
        float currentFres = Mathf.Lerp(fres1, fres2, lerp);
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_FresnelThreshold", currentFres);

        slider.value = currentFres;
        sliderText.text = currentFres.ToString("0.00");
    }

    float previousFresnel = 0f;
    float previewTimer = 0f;
    public void PreviewSlider()
    {
        
        if (refToShape.CurrentShape == null)
            return;

        float sliderValue = 1f;
        sliderValue = slider.value;
        previousFresnel = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetFloat("_FresnelThreshold");
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_FresnelThreshold", sliderValue);
        sliderText.text = sliderValue.ToString("0.00");
    }

    public void AddKeyframe()
    {
        float fres = slider.value;
        PartFile.GetInstance().KeyFrames.AddKeyFresnel(KeyframeMainWindow.SelectedFrame, fres);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
