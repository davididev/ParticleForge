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
        public enum COLOR { Red, Green, Blue, Alpha};
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
    public Slider alphaSlider;
    public TMPro.TMP_InputField alphaSliderLabel;
    public Image resultColor;
    private Color CurrentColor;
    public static Color KeyframeColor = Color.white;
    private Color LastKeyframeColor;
    public StartingShapeHolder refToShape;

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
        CurrentColor = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetColor("_DiffuseColor");
        Debug.Log("Keyframe color: " + CurrentColor.ToString());
        LastKeyframeColor = CurrentColor;
        SetSliderValues();
        SetTextBoxValues();
        UpdateColorSliderGradients();
        UpdateRefColor();
        Init = true;
    }

    /// <summary>
    /// Should be called by SetUIValues or when the text boxes change
    /// </summary>
    void SetSliderValues()
    {
        ColorSliders[0].SliderRef.value = CurrentColor.r;
        ColorSliders[1].SliderRef.value = CurrentColor.g;
        ColorSliders[2].SliderRef.value = CurrentColor.b;

        alphaSlider.value = CurrentColor.a;
    }

    /// <summary>
    /// Should be called by SetUIValues or when the sliders change
    /// </summary>
    void SetTextBoxValues()
    {
        ColorSliders[0].ColorText.text = Mathf.Round(CurrentColor.r * 255f).ToString();
        ColorSliders[1].ColorText.text = Mathf.Round(CurrentColor.g * 255f).ToString();
        ColorSliders[2].ColorText.text = Mathf.Round(CurrentColor.b * 255f).ToString();
        alphaSliderLabel.text = Mathf.Round(CurrentColor.a * 255f).ToString();
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
        if (Init == false)
            return;
        CurrentColor.r = ColorSliders[0].SliderRef.value;
        CurrentColor.g = ColorSliders[1].SliderRef.value;
        CurrentColor.b = ColorSliders[2].SliderRef.value;
        CurrentColor.a = alphaSlider.value;
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
        CurrentColor.a = int.Parse(alphaSliderLabel.text) / 255f;
        SetSliderValues();
        UpdateColorSliderGradients();
        UpdateRefColor();
    }

    public void AddKeyframe()
    {
        
        PartFile.GetInstance().KeyFrames.AddKeyframeColor(KeyframeMainWindow.SelectedFrame, CurrentColor);
    }

    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        Init = false;
        Invoke("SetUIValues", Time.deltaTime * 2f);  //Give it a couple of framesteps to update so blue works properly.
    }

    void UpdateRefColor()
    {
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_DiffuseColor", CurrentColor);
    }
}
