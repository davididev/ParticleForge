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
            int r = int.Parse(Red.ColorText.text);
            r = Mathf.Clamp(r, 0, 255);
            Red.ColorText.text = r.ToString();
            Red.SliderRef.value = (float)r / 255f;

            int g = int.Parse(Green.ColorText.text);
            g = Mathf.Clamp(g, 0, 255);
            Green.ColorText.text = g.ToString();
            Green.SliderRef.value = (float) g / 255f;


            int b = int.Parse(Blue.ColorText.text);
            b = Mathf.Clamp(b, 0, 255);
            Blue.ColorText.text = b.ToString();
            Blue.SliderRef.value = (float)b / 255f;
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
