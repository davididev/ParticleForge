using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDiffuseColorPanel : MonoBehaviour
{
    [System.Serializable]
    public class IndivColor
    {
        public Image ColorBackground;
        public Slider SliderRef;
        public TMPro.TMP_InputField ColorText;
        public enum COLOR { Red, Green, Blue};
        public COLOR Identity = COLOR.Red;
        public void UpdateColors(Color current)
        {
            //temps are gradients for this slider, basically what the color would be if slider is all the
            //way to the left or right
            Color ctempLeft = current;
            Color ctempRight = current;
            if (Identity == COLOR.Red)
            {
                ctempLeft.r = 0f;
                ctempRight.r = 1f;
            }
            if (Identity == COLOR.Green)
            {
                ctempLeft.g = 0f;
                ctempRight.g = 1f;
            }
            if (Identity == COLOR.Blue)
            {
                ctempLeft.b = 0f;
                ctempRight.b = 1f;
            }

            ColorBackground.material.SetColor("_ColorLeft", ctempLeft);
            ColorBackground.material.SetColor("_ColorRight", ctempRight);
        }

    }

    [SerializeField]
    public IndivColor[] ColorSliders;
    public Image resultColor;
    private Color CurrentColor;
    public static Color KeyframeColor = Color.white;
    private Color LastKeyframeColor;

    // Start is called before the first frame update
    void OnEnable()
    {
        //CurrentColor = Color.white;
        CurrentColor = KeyframeColor;
        SetUIValues();
    }

    /// <summary>
    /// Should be called whenever CurrentColor changes
    /// </summary>
    void SetUIValues()
    {
        SetSliderValues();
        SetTextBoxValues();
        UpdateColorSliderGradients();
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
        for(int i = 0; i < ColorSliders.Length; i++)
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
        CurrentColor.r = ColorSliders[0].SliderRef.value;
        CurrentColor.g = ColorSliders[1].SliderRef.value;
        CurrentColor.b = ColorSliders[2].SliderRef.value;
        SetTextBoxValues();
        UpdateColorSliderGradients();
    }

    /// <summary>
    /// Should be called whenever a text box (color) has changed
    /// </summary>
    public void OnUpdateColorTextBox()
    {
        CurrentColor.r = int.Parse(ColorSliders[0].ColorText.text) / 255f;
        CurrentColor.g = int.Parse(ColorSliders[1].ColorText.text) / 255f;
        CurrentColor.b = int.Parse(ColorSliders[2].ColorText.text) / 255f;
        SetSliderValues();
        UpdateColorSliderGradients();
    }

    public void AddKeyframe()
    {
        
        PartFile.GetInstance().KeyFrames.AddKeyframeColor(KeyframeMainWindow.SelectedFrame, CurrentColor);
    }

    // Update is called once per frame
    void Update()
    {
        if(LastKeyframeColor != KeyframeColor)  //The color was updated by the timeline
        {
            CurrentColor = KeyframeColor;
            LastKeyframeColor = KeyframeColor;
            SetUIValues();
        }
    }
}
