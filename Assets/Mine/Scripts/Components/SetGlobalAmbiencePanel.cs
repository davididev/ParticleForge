using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGlobalAmbiencePanel : MonoBehaviour
{
    [SerializeField]
    public SetDiffuseColorPanel.IndivColor[] ColorSliders;
    public Image resultColor;
    private Color CurrentColor;
    public static Color KeyframeColor = Color.white;
    private Color LastKeyframeColor;

    bool Init = false;  //Don't call slider onupdate until two framesteps have passed

    // Start is called before the first frame update
    void OnEnable()
    {
        //CurrentColor = Color.white;
        //Invoke("SetUIValues", Time.deltaTime * 2f);  //Give it a couple of framesteps to update so blue works properly.
    }

    /// <summary>
    /// Should be called whenever CurrentColor changes
    /// </summary>
    void SetUIValues()
    {
        CurrentColor = RenderSettings.ambientLight;
        Debug.Log("Keyframe color: " + CurrentColor.ToString());
        LastKeyframeColor = CurrentColor;
        SetSliderValues();
        SetTextBoxValues();
        UpdateColorSliderGradients();
        Init = true;
        UpdateRefColor();
    }

    /// <summary>
    /// Should be called by SetUIValues or when the text boxes change
    /// </summary>
    void SetSliderValues()
    {
        ColorSliders[0].SliderRef.value = CurrentColor.r;
        ColorSliders[1].SliderRef.value = CurrentColor.g;
        ColorSliders[2].SliderRef.value = CurrentColor.b;
    }

    /// <summary>
    /// Should be called by SetUIValues or when the sliders change
    /// </summary>
    void SetTextBoxValues()
    {
        ColorSliders[0].ColorText.text = Mathf.Round(CurrentColor.r * 255f).ToString();
        ColorSliders[1].ColorText.text = Mathf.Round(CurrentColor.g * 255f).ToString();
        ColorSliders[2].ColorText.text = Mathf.Round(CurrentColor.b * 255f).ToString();
    }

    /// <summary>
    /// Should be called whenver we need to set the colors on the gradients
    /// </summary>
    void UpdateColorSliderGradients()
    {
        for (int i = 0; i < ColorSliders.Length; i++)
        {
            ColorSliders[i].UpdateColors(CurrentColor);
        }

        resultColor.color = CurrentColor;

    }

    /// <summary>
    /// Should be called whenever a slider value changes
    /// </summary>
    public void OnUpdateSlider()
    {
        if (Init == false)
            return;
        CurrentColor.r = ColorSliders[0].SliderRef.value;
        CurrentColor.g = ColorSliders[1].SliderRef.value;
        CurrentColor.b = ColorSliders[2].SliderRef.value;
        SetTextBoxValues();
        UpdateColorSliderGradients();
        UpdateRefColor();
    }

    /// <summary>
    /// Should be called whenever a text box (color) has changed
    /// </summary>
    public void OnUpdateColorTextBox()
    {
        if (Init == false)
            return;
        CurrentColor.r = int.Parse(ColorSliders[0].ColorText.text) / 255f;
        CurrentColor.g = int.Parse(ColorSliders[1].ColorText.text) / 255f;
        CurrentColor.b = int.Parse(ColorSliders[2].ColorText.text) / 255f;
        SetSliderValues();
        UpdateColorSliderGradients();
        UpdateRefColor();
    }

    public void AddKeyframe()
    {

        PartFile.GetInstance().KeyFrames.AddKeyframeAmbientLightColor(KeyframeMainWindow.SelectedFrame, CurrentColor);
    }

    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        Init = false;
        Invoke("SetUIValues", Time.deltaTime * 2f);  //Give it a couple of framesteps to update so blue works properly.
    }

    void UpdateRefColor()
    {
        RenderSettings.ambientLight = CurrentColor;
    }
}
