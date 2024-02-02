using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyframeMainWindow : MonoBehaviour
{

    public static bool PreviewEffect = false;
    public Light LightRef;
    public RectTransform CursorPosition;
    public RectTransform BackgroundOfTimeline;
    public StartingShapeHolder refToShape;
    public GameObject MarkerPrefab;
    private GameObject[] MarkerGameObjects;
    public GameObject[] EditingWindows;
    public TMPro.TMP_Dropdown DropdownObjectMode;
    public TMPro.TextMeshProUGUI FrameNumberText, FrameTypeText;
    public static int SelectedFrame = 1;
    public enum OBJECT_MODE { Vertex, Rotation, Position, NoiseOffset, SetColor, Fresnel, Lighting1, Lighting2, Lighting3};
    public OBJECT_MODE CurrentMode = OBJECT_MODE.Rotation;
    // Start is called before the first frame update
    void Start()
    {
        MarkerGameObjects = new GameObject[64];
        MarkerGameObjects[0] = MarkerPrefab;
        for(int i = 1; i < 64; i++)
        {
            MarkerGameObjects[i] = GameObject.Instantiate(MarkerPrefab, MarkerPrefab.transform.parent);
            Vector3 localPos = MarkerGameObjects[i].transform.localPosition;
            localPos.x = 12f * i;
            MarkerGameObjects[i].transform.localPosition = localPos;
            MarkerGameObjects[i].transform.SetAsFirstSibling();
            MarkerGameObjects[i].SetActive(false);
        }

        Vector2 delta = BackgroundOfTimeline.sizeDelta;
        delta.x = 12f * PartFile.GetInstance().FrameCount;
        BackgroundOfTimeline.sizeDelta = delta;

        UpdateDropdown();
    }

    public static KeyframeMainWindow GetInstance()
    {
        return GameObject.FindWithTag("MainWindow").GetComponent<KeyframeMainWindow>();
    }

    /// <summary>
    /// When a new frame is selected, or when a keyframe is updated (to update rotation, shape, color, etc)
    /// </summary>
    public void RefreshObjectState()
    {
        //Add functionality here 
        for (int i = 0; i < EditingWindows.Length; i++)
        {
            if (EditingWindows[i].activeInHierarchy == true)
                EditingWindows[i].SendMessage("RefreshUI", SendMessageOptions.DontRequireReceiver);
        }

        if (refToShape.CurrentShape == null)
            return;

        if (CurrentMode == KeyframeMainWindow.OBJECT_MODE.Vertex)  //Vertex mode- do default change the euler angles
        {
            if (refToShape.CurrentShape != null)
                refToShape.CurrentShape.transform.localEulerAngles = new Vector3(-90f, 90f, -90f);
            
            //Show wireframe
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 1f);

            //Todo- make moveable points
        }
        else  //Anything other than vertex mode
        {
            //Hide wireframe
            float lerp = 0f;  //This will be used by all functions
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 0f);

            //Set rotation
            List<KeyframeData<Vector3>> tempData = PartFile.GetInstance().KeyFrames.RotationKeyframes;
            Vector3 rot1 = Vector3.zero;
            Vector3 rot2 = Vector3.zero;
            KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData);
            Vector3 currentFrame = Vector3.Lerp(rot1, rot2, lerp);        
            if (refToShape.CurrentShape != null)
                refToShape.CurrentShape.transform.localEulerAngles = currentFrame;

            //Set position
            List<KeyframeData<Vector2>> tempData6 = PartFile.GetInstance().KeyFrames.PositionKeyframes;
            Vector2 pos1 = Vector2.zero;
            Vector2 pos2 = Vector2.zero;
            KeyframeData<Vector2>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out pos1, out pos2, out lerp, tempData6);
            Vector2 currentFramePos = Vector3.Lerp(pos1, pos2, lerp);
            TransformArrow.NewWorldPosition = new Vector3(currentFramePos.x, currentFramePos.y, 0f);
            refToShape.transform.position = TransformArrow.NewWorldPosition;

            //Set fresnel threshold
            List<KeyframeData<float>> tempData2 = PartFile.GetInstance().KeyFrames.FresnelKeyframes;
            float fres1 = 0f;
            float fres2 = 0f;
            KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData2);
            float currentFres = Mathf.Lerp(fres1, fres2, lerp);
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_FresnelThreshold", currentFres);

            //Set light settings (rotation)
            List<KeyframeData<Vector3>> tempData3 = PartFile.GetInstance().KeyFrames.DirectionalLightRotationKeyframes;
            KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData3);
            Vector3 CurrentLightRotation = Vector3.Lerp(rot1, rot2, lerp);
            LightRef.transform.localEulerAngles = CurrentLightRotation;

            //Set light settings (intensity dir)
            List<KeyframeData<float>> tempData4 = PartFile.GetInstance().KeyFrames.DirectionalLightIntensityKeys;
            KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData4);
            LightRef.intensity = Mathf.Lerp(fres1, fres2, lerp);

            //Set light settings (scene dir)
            List<KeyframeData<float>> tempData5 = PartFile.GetInstance().KeyFrames.SceneLightIntensityKeys;
            KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData5);
            RenderSettings.ambientLight = Color.white * Mathf.Lerp(fres1, fres2, lerp);

            //Not in vertex mode- close the wireframe
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 0f);
        }
    }

    /// <summary>
    /// Called whenever a new keyframe is added or dropdown is changed; redraws the markers
    /// </summary>
    public void UpdateKeyframes()
    {
        int[] keyframes = new int[1];
        if (CurrentMode == OBJECT_MODE.Rotation)
            keyframes = KeyframeData<Vector3>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.RotationKeyframes);
        if (CurrentMode == OBJECT_MODE.Position)
            keyframes = KeyframeData<Vector2>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.PositionKeyframes);
        if (CurrentMode == OBJECT_MODE.NoiseOffset)
            keyframes = KeyframeData<Vector2>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.NoiseTextureKeyframes);
        if (CurrentMode == OBJECT_MODE.SetColor)
            keyframes = KeyframeData<Color>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.ColorKeyframes);
        if (CurrentMode == OBJECT_MODE.Vertex)
            keyframes = KeyframeData<ShapeData>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.ShapeKeyframes);
        if (CurrentMode == OBJECT_MODE.Fresnel)
            keyframes = KeyframeData<float>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.FresnelKeyframes);
        int i = 0; //i is the number of keyframes in the current mode
        for(int x = 1; x < 64; x++)  //x is the number of total markers of all possible keyframes
        {
            if(i >= keyframes.Length)
            {
                MarkerGameObjects[x-1].SetActive(false);
            }
            else
            {
                if (keyframes[i] == x)
                {
                    MarkerGameObjects[x-1].SetActive(true);
                    i++;
                }
                else
                {
                    MarkerGameObjects[x-1].SetActive(false);
                }
            }
            
                
        }
        //RefreshObjectState();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrameNumber();
    }

    void UpdateFrameNumber()
    {
        bool changedPos = false;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            changedPos = true;
            SelectedFrame++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            changedPos = true;
            SelectedFrame--;
        }
        SelectedFrame = Mathf.Clamp(SelectedFrame, 1, PartFile.GetInstance().FrameCount);

        if(changedPos == true)
        {
            Vector3 localPos = CursorPosition.localPosition;
            localPos.x = (SelectedFrame - 1) * 12.0f;
            CursorPosition.localPosition = localPos;

            FrameNumberText.text = "F: " + SelectedFrame + " / " + PartFile.GetInstance().FrameCount;
            RefreshObjectState();
        }

        
    }

    public void UpdateDropdown()
    {
        
        int id = DropdownObjectMode.value;
        if (id == 0)
            DropdownRotationSelected();
        if (id == 1)
            DropdownFresnelSelected(); 
        if (id == 2)
            DropdownPositionSelected(); 
        if (id == 3)
            DropdownColorSelected();
        if (id == 4)
            DropdownNoiseOffsetSelected();
        if (id == 5)
            DropdownVertexSelected();
        if (id == 6)
            DropdownLightingDetected1();
        if (id == 7)
            DropdownLightingDetected2();
        if (id == 8)
            DropdownLightingDetected3();

        for (int i = 0; i < EditingWindows.Length; i++)
        {
            EditingWindows[i].SetActive(i == id);
        }
        UpdateKeyframes();
        RefreshObjectState();
    }

    public void DropdownRotationSelected()
    {
        CurrentMode = OBJECT_MODE.Rotation;
        FrameTypeText.text = "Keyframes: Rotation";
    }

    public void DropdownPositionSelected()
    {
        CurrentMode = OBJECT_MODE.Position;
        FrameTypeText.text = "Keyframes: Position";
    }

    public void DropdownNoiseOffsetSelected()
    {
        CurrentMode = OBJECT_MODE.NoiseOffset;
        FrameTypeText.text = "Keyframes: Noise UV Offset";
    }

    public void DropdownColorSelected()
    {
        CurrentMode = OBJECT_MODE.SetColor;
        FrameTypeText.text = "Keyframes: Color";
    }

    public void DropdownFresnelSelected()
    {
        CurrentMode = OBJECT_MODE.Fresnel;
        FrameTypeText.text = "Keyframes: Fresnel";
    }

    public void DropdownVertexSelected()
    {
        CurrentMode = OBJECT_MODE.Vertex;
        FrameTypeText.text = "Keyframes: Shape";
    }

    public void DropdownLightingDetected1()
    {
        CurrentMode = OBJECT_MODE.Lighting1;
        FrameTypeText.text = "Keyframes: Lighting Direction";
    }
    public void DropdownLightingDetected2()
    {
        CurrentMode = OBJECT_MODE.Lighting2;
        FrameTypeText.text = "Keyframes: Lighting Intensity";
    }
    public void DropdownLightingDetected3()
    {
        CurrentMode = OBJECT_MODE.Lighting3;
        FrameTypeText.text = "Keyframes: Ambient Intensity";
    }
}
