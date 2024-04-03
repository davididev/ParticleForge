using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGlowValuePanel : MonoBehaviour
{
    [System.Serializable]
    public class ColorHolder
    {
        [SerializeField]
        SetDiffuseColorPanel.IndivColor Red;
        [SerializeField]
        SetDiffuseColorPanel.IndivColor Green;
        [SerializeField]
        SetDiffuseColorPanel.IndivColor Blue;

        [SerializeField]
        public Image ColorOutput;

        private Color LastKeyframeColor = Color.black;

        /// <summary>
        /// Get the color and update the output
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            Color c = Color.black;
            c.r = Red.SliderRef.value;
            c.g = Green.SliderRef.value;
            c.b = Blue.SliderRef.value;
            c.a = 1f;
            ColorOutput.color = c;
            LastKeyframeColor = c;
            return c;
        }

        /// <summary>
        /// Set the color from outside the sliders.
        /// </summary>
        /// <param name="c"></param>
        public void SetColor(Color c)
        {
            Red.SliderRef.value = c.r;
            Green.SliderRef.value = c.g;
            Blue.SliderRef.value = c.b;
            Red.UpdateColors(c);
            Green.UpdateColors(c);
            Blue.UpdateColors(c);

            ColorOutput.color = c;
            LastKeyframeColor = c;
            OnUpdateSlider();  //Refresh the text boxes
        }

        public Color GetLastColor()
        {
            return LastKeyframeColor;
        }

        public void OnUpdateSlider()
        {
            Red.ColorText.text = Mathf.Round(Red.SliderRef.value * 255f).ToString();
            Green.ColorText.text = Mathf.Round(Green.SliderRef.value * 255f).ToString();
            Blue.ColorText.text = Mathf.Round(Blue.SliderRef.value * 255f).ToString();
            
        }

        public void OnUpdateTextBoxes()
        {
            float r = float.Parse(Red.ColorText.text.ToString()) / 255f;
            if (r < 0f)
                Red.ColorText.text = "0";
            if (r > 1f)
                Red.ColorText.text = "255";

            r = Mathf.Clamp(r, 0f, 1f);
            Red.SliderRef.value = r;

            float g = float.Parse(Green.ColorText.text.ToString()) / 255f;
            if (g < 0f)
                Green.ColorText.text = "0";
            if (g > 1f)
                Green.ColorText.text = "255";

            g = Mathf.Clamp(g, 0f, 1f);
            Green.SliderRef.value = g;


            float b = float.Parse(Blue.ColorText.text.ToString()) / 255f;
            if (b < 0f)
                Blue.ColorText.text = "0";
            if (b > 1f)
                Blue.ColorText.text = "255";

            b = Mathf.Clamp(b, 0f, 1f);
            Blue.SliderRef.value = b;
        }
    }

    [SerializeField]
    private ColorHolder ColorInner, ColorOuter;

    public StartingShapeHolder refToShape;
    public Slider InnerFresnel, OuterFresnel;
    public TMPro.TextMeshProUGUI InnerFresnelText, OuterFresnelText;

    public static Color KeyframeColor1 = Color.black;
    public static Color KeyframeColor2 = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    /// <summary>
    /// Should be called whenever The inspector gets this object
    /// </summary>
    void SetUIValues()
    {
        RefreshUI();
        Color c1 = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetColor("_InnerRimColor");
        ColorInner.SetColor(c1);
        Color c2 = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetColor("_OuterRimColor");
        ColorOuter.SetColor(c2);
        InnerFresnel.value = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetFloat("_InnerRimThreshold");
        OuterFresnel.value = refToShape.CurrentShape.GetComponent<MeshRenderer>().material.GetFloat("_OuterRimThreshold");
        OnUpdateFresnelSliders();
        RedrawVars();
        Init = true;
    }

    bool Init = true;

    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        Init = false;
        Invoke("SetUIValues", Time.deltaTime * 2f);  //Give it a couple of framesteps to update so blue works properly.
        
    }

    /// <summary>
    /// Called whenever a UI value changes, to redraw the glow of the shape
    /// </summary>
    void RedrawVars()
    {
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_InnerRimColor", ColorInner.GetColor());
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_OuterRimColor", ColorOuter.GetColor());
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_InnerRimThreshold", InnerFresnel.value);
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_OuterRimThreshold", OuterFresnel.value);
    }

    /// <summary>
    /// Should be called by the UI whenever a slider changes
    /// </summary>
    public void OnUpdateColorSliders()
    {
        if (Init == false)
            return;
        ColorInner.OnUpdateSlider();
        ColorOuter.OnUpdateSlider();

        RedrawVars();
    }

    /// <summary>
    /// Should be called by the UI whenever a fresnel slider changes
    /// </summary>
    public void OnUpdateFresnelSliders()
    {
        if (Init == false)
            return;
        float f1 = InnerFresnel.value;
        float f2 = OuterFresnel.value;
        InnerFresnelText.text = f1.ToString("0.00");
        OuterFresnelText.text = f2.ToString("0.00");
        RedrawVars();
    }

    /// <summary>
    /// Should be called by the UI whenever a text box changes
    /// </summary>
    public void OnUpdateColorTextBox()
    {
        if (Init == false)
            return;
        ColorInner.OnUpdateTextBoxes();
        ColorOuter.OnUpdateTextBoxes();
        RedrawVars();
    }




    public void AddKeyframe()
    {
        GlowData newEntry = new GlowData();
        newEntry.innerColor = ColorInner.GetColor();
        newEntry.outerColor = ColorOuter.GetColor();
        newEntry.innerThreshold = InnerFresnel.value;
        newEntry.outerThreshold = OuterFresnel.value;
        PartFile.GetInstance().KeyFrames.AddKeyframeGlow(KeyframeMainWindow.SelectedFrame, newEntry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
