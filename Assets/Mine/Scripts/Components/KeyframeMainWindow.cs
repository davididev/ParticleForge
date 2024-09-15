using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SkywardRay.FileBrowser;

public class KeyframeMainWindow : MonoBehaviour, IPointerClickHandler
{
    public SkywardFileBrowser fileBrowser;
    public static bool PreviewEffect = false;
    public Light LightRef;
    public RectTransform CursorPosition;
    public RectTransform BackgroundOfTimeline;
    public StartingShapeHolder refToShape;
    public GameObject MarkerPrefab;
    private GameObject[] MarkerGameObjects;
    public GameObject[] EditingWindows;
    public Button DeleteKeyframeButton;
    public TMPro.TMP_Dropdown DropdownObjectMode;
    public TMPro.TextMeshProUGUI FrameNumberText, FrameTypeText, SaveFileText;
    public static int SelectedFrame = 1;
    float saveFileTimer = 0.0f;
    private bool isPlayingPreview = false;
    private float playingPreviewTimer = 0f;
    public enum OBJECT_MODE { Vertex, Rotation, Position, NoiseOffset, SetColor, Fresnel, Fresnel2, Lighting1, Lighting2, Lighting3, Lighting4, Glow, Render, NoiseDirectory};
    public OBJECT_MODE CurrentMode = OBJECT_MODE.Rotation;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)  //Moved over here so the timeline has a chance to generate
        {
            PartFile.GetInstance().LoadFile(PlayerPrefs.GetString("LastFile"));
        }
        Invoke("LoadNoise", 0.1f);


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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(pointerEventData.pressPosition.x, pointerEventData.pressPosition.y, 0f));
        Vector3 localPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        

        int frame = Mathf.FloorToInt(localPos.x / 12f);
        SelectedFrame = frame;
        if (SelectedFrame > PartFile.GetInstance().FrameCount)
            SelectedFrame = PartFile.GetInstance().FrameCount;

        Vector3 localPos2 = CursorPosition.localPosition;
        localPos2.x = (SelectedFrame - 1) * 12.0f;
        CursorPosition.localPosition = localPos2;

        FrameNumberText.text = "F: " + SelectedFrame + " / " + PartFile.GetInstance().FrameCount;
        RefreshObjectState();
    }


    void LoadNoise()
    {
        //Moving these to update object state
        //Texture2D Noise = PartFile.GetInstance().LoadNoise(KeyframeMainWindow.SelectedFrame);
        //refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetTexture("_Noise", Noise);
        RefreshObjectState();  //File loaded, let's go
    }

    public void SaveFileButton()
    {
        try
        {
            PartFile.GetInstance().SaveFile(PlayerPrefs.GetString("LastFile"));
            saveFileTimer = 2.5f;
            SaveFileText.text = "<color=yellow>File saved.";
        }
        catch(System.Exception e)
        {
            saveFileTimer = 2.5f;
            SaveFileText.text = "<color=red>Failed to save.  Try again.";
        }
        
    }

    public static KeyframeMainWindow GetInstance()
    {
        return GameObject.FindWithTag("MainWindow").GetComponent<KeyframeMainWindow>();
    }

    /// <summary>
    /// This is going to be called by Keyframe editor too, loading noise
    /// </summary>
    public void LoadNoiseTextureButton()
    {
        string dir = DirectoryHelper.GetDirectoryOfFile(PlayerPrefs.GetString("LastFile"));
        fileBrowser.OpenFile(dir, NoiseLoaded, new string[] { ".png", ".jpg" });
    }

    void NoiseLoaded(string[] output)
    {
        string dir1 = PlayerPrefs.GetString("LastFile");  //file we are currently in
        dir1 = DirectoryHelper.GetDirectoryOfFile(dir1);
        
        string dir2 = output[0];
        //PartFile.GetInstance().NoiseDirectory = DirectoryHelper.GetRelativePath(dir1, dir2);
        PartFile.GetInstance().KeyFrames.AddNoiseTextureFile(KeyframeMainWindow.SelectedFrame, DirectoryHelper.GetRelativePath(dir1, dir2));
        Texture2D Noise = PartFile.GetInstance().LoadNoise(KeyframeMainWindow.SelectedFrame);
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetTexture("_Noise", Noise);
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
        Texture2D Noise = PartFile.GetInstance().LoadNoise(KeyframeMainWindow.SelectedFrame);
        refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetTexture("_Noise", Noise);
        float lerp = 0f;  //This will be used by all functions

        //In both modes, vertex and object, show the mesh vertex positions
        List<KeyframeData<ShapeData>> tempData0 = PartFile.GetInstance().KeyFrames.ShapeKeyframes;
        ShapeData shape1 = new ShapeData();
        ShapeData shape2 = new ShapeData();
        KeyframeData<ShapeData>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out shape1, out shape2, out lerp, tempData0);
        ShapeData currentShape = ShapeData.Lerp(shape1, shape2, lerp);

        Vector3[] points = refToShape.GetVertices();
        List<Vector3>.Enumerator e0 = currentShape.Vertices.GetEnumerator();
        int pt = 0;
        while(e0.MoveNext())
        {
            points[pt] = e0.Current;
            pt++;
        }

        refToShape.SetVertices(points);


        if (CurrentMode == KeyframeMainWindow.OBJECT_MODE.Vertex)  //Vertex mode- do default change the euler angles
        {
            if (refToShape.CurrentShape != null)
                refToShape.CurrentShape.transform.localEulerAngles = new Vector3(-90f, 90f, -90f);
            
            //Show wireframe
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 1f);

        }
        else  //Anything other than vertex mode
        {
            //Hide wireframe
            
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 0f);

            //Set rotation
            List<KeyframeData<Vector3>> tempData = PartFile.GetInstance().KeyFrames.RotationKeyframes;
            Vector3 rot1 = Vector3.zero;
            Vector3 rot2 = Vector3.zero;
            
            KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData);

            Quaternion q1 = Quaternion.Euler(rot1);
            Quaternion q2 = Quaternion.Euler(rot2);
            Quaternion currentFrame = Quaternion.Lerp(q1, q2, lerp);        
            if (refToShape.CurrentShape != null)
                refToShape.CurrentShape.transform.rotation = currentFrame;

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

            //Set inner fresnel threshold
            List<KeyframeData<float>> tempData10 = PartFile.GetInstance().KeyFrames.Fresnel2Keyframes;
            fres1 = 0f;
            fres2 = 0f;
            KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData10);
            currentFres = Mathf.Lerp(fres1, fres2, lerp);
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_InnerFresnelThreshold", currentFres);

            //Set light settings (rotation)
            List<KeyframeData<Vector3>> tempData3 = PartFile.GetInstance().KeyFrames.DirectionalLightRotationKeyframes;
            KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData3);
            Vector3 CurrentLightRotation = Vector3.Lerp(rot1, rot2, lerp);
            LightRef.transform.localEulerAngles = CurrentLightRotation;

            //Set light settings (intensity dir)
            List<KeyframeData<float>> tempData4 = PartFile.GetInstance().KeyFrames.DirectionalLightIntensityKeys;
            KeyframeData<float>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out fres1, out fres2, out lerp, tempData4);
            LightRef.intensity = Mathf.Lerp(fres1, fres2, lerp);

            //Set light settings (scene)
            Color c1;
            Color c2;
            List<KeyframeData<Color>> tempData5 = PartFile.GetInstance().KeyFrames.SceneLightColorKeys;
            KeyframeData<Color>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out c1, out c2, out lerp, tempData5);
            RenderSettings.ambientLight = Color.Lerp(c1, c2, lerp);

            //Set dir light settings 
            List<KeyframeData<Color>> tempData9 = PartFile.GetInstance().KeyFrames.DirectionalLightColorKeys;
            KeyframeData<Color>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out c1, out c2, out lerp, tempData9);
            LightRef.color = Color.Lerp(c1, c2, lerp);

            //Set Diffuse Colors
            List<KeyframeData<Color>> tempData7 = PartFile.GetInstance().KeyFrames.ColorKeyframes;
            
            KeyframeData<Color>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out c1, out c2, out lerp, tempData7);
            //RenderSettings.ambientLight = Mathf.Lerp(fres1, fres2, lerp);
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_DiffuseColor", Color.Lerp(c1, c2, lerp));
            SetDiffuseColorPanel.KeyframeColor = Color.Lerp(c1, c2, lerp);
            //Not in vertex mode- close the wireframe
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_WireframeThreshold", 0f);

            //Set texture offset
            List<KeyframeData<Vector2>> tempData8 = PartFile.GetInstance().KeyFrames.NoiseTextureKeyframes;
            Vector2 offset1 = Vector2.zero;
            Vector2 offset2 = Vector2.zero;
            KeyframeData<Vector2>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out offset1, out offset2, out lerp, tempData8);
            Vector2 currentOffset = Vector2.Lerp(offset1, offset2, lerp);
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetTextureOffset("_Noise", currentOffset);

            //Set glow settings
            List<KeyframeData<GlowData>> tempData11 = PartFile.GetInstance().KeyFrames.GlowKeys;
            GlowData g1;
            GlowData g2;
            KeyframeData<GlowData>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out g1, out g2, out lerp, tempData11);
            //RenderSettings.ambientLight = Mathf.Lerp(fres1, fres2, lerp);
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_InnerRimColor", Color.Lerp(g1.innerColor, g2.innerColor, lerp));
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetColor("_OuterRimColor", Color.Lerp(g1.outerColor, g2.outerColor, lerp));
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_InnerRimThreshold", Mathf.Lerp(g1.innerThreshold, g2.innerThreshold, lerp));
            refToShape.CurrentShape.GetComponent<MeshRenderer>().material.SetFloat("_OuterRimThreshold", Mathf.Lerp(g1.outerThreshold, g2.outerThreshold, lerp));
            //SetGlowValuePanel.CurrentGlowData = (GlowData.Lerp(g1, g2, lerp))



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
        if (CurrentMode == OBJECT_MODE.Fresnel2)
            keyframes = KeyframeData<float>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.Fresnel2Keyframes);
        if (CurrentMode == OBJECT_MODE.Lighting1)
            keyframes = KeyframeData<Vector3>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.DirectionalLightRotationKeyframes);
        if (CurrentMode == OBJECT_MODE.Lighting2)
            keyframes = KeyframeData<float>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.DirectionalLightIntensityKeys);
        if (CurrentMode == OBJECT_MODE.Lighting3)
            keyframes = KeyframeData<Color>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.DirectionalLightColorKeys); 
        if (CurrentMode == OBJECT_MODE.Lighting4)
            keyframes = KeyframeData<Color>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.SceneLightColorKeys);
        if (CurrentMode == OBJECT_MODE.Glow)
            keyframes = KeyframeData<GlowData>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.GlowKeys);
        if (CurrentMode == OBJECT_MODE.NoiseDirectory)
            keyframes = KeyframeData<string>.GetKeyframeIDS(PartFile.GetInstance().KeyFrames.NoiseFileKeyframes);
        int i = 0; //i is the number of keyframes in the current mode
        for(int x = 1; x < 65; x++)  //x is the number of total markers of all possible keyframes
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
        if(saveFileTimer > 0f)
        {
            saveFileTimer -= Time.deltaTime;
            if (saveFileTimer <= 0f)
                SaveFileText.text = "";
        }
        UpdateFrameNumber();
        if (Input.GetKeyDown(KeyCode.Space))
            OnPlayButtonPressed();

        IsButtonEnabled();
        if (Input.GetKeyDown(KeyCode.Delete) && DeleteKeyframeButton.interactable == true)
            OnKeyframeDeletePressed();
    }

    void IsButtonEnabled()
    {
        int x = SelectedFrame - 1;
        if(x == 0) //Cannot delete the first keyframe
        {
            DeleteKeyframeButton.interactable = false;
            return;
        }
        if (MarkerGameObjects[x].activeInHierarchy == true)
            DeleteKeyframeButton.interactable = true;
        else
            DeleteKeyframeButton.interactable = false;
    }

    public void OnKeyframeDeletePressed()
    {
        if (CurrentMode == OBJECT_MODE.Render)
            return;
        int x = SelectedFrame - 1;
        if(CurrentMode == OBJECT_MODE.Vertex)
        {
            List<KeyframeData<ShapeData>> list = PartFile.GetInstance().KeyFrames.ShapeKeyframes;
            for(int i = 0; i < list.Count; i++)
            {
                Debug.Log(i + ": Current Frame is " + SelectedFrame + "; at frame " + list[i].FrameNum);
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }

            }
        }

        if (CurrentMode == OBJECT_MODE.NoiseOffset)
        {
            List<KeyframeData<Vector2>> list = PartFile.GetInstance().KeyFrames.NoiseTextureKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }
            }
        }

        if (CurrentMode == OBJECT_MODE.Position)
        {
            List<KeyframeData<Vector2>> list = PartFile.GetInstance().KeyFrames.PositionKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }
            }
        }

        if (CurrentMode == OBJECT_MODE.Rotation)
        {
            List<KeyframeData<Vector3>> list = PartFile.GetInstance().KeyFrames.RotationKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }
            }
        }

        if (CurrentMode == OBJECT_MODE.SetColor)
        {
            List<KeyframeData<Color>> list = PartFile.GetInstance().KeyFrames.ColorKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }
            }
        }

        if (CurrentMode == OBJECT_MODE.Fresnel)
        {
            List<KeyframeData<float>> list = PartFile.GetInstance().KeyFrames.FresnelKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                    { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.Fresnel2)
        {
            List<KeyframeData<float>> list = PartFile.GetInstance().KeyFrames.Fresnel2Keyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }

        if (CurrentMode == OBJECT_MODE.Lighting1)
        {
            List<KeyframeData<Vector3>> list = PartFile.GetInstance().KeyFrames.DirectionalLightRotationKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.Lighting2)
        {
            List<KeyframeData<float>> list = PartFile.GetInstance().KeyFrames.DirectionalLightIntensityKeys;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.Lighting3)
        {
            List<KeyframeData<Color>> list = PartFile.GetInstance().KeyFrames.DirectionalLightColorKeys;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.Lighting4)
        {
            List<KeyframeData<Color>> list = PartFile.GetInstance().KeyFrames.SceneLightColorKeys;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.Glow)
        {
            List<KeyframeData<GlowData>> list = PartFile.GetInstance().KeyFrames.GlowKeys;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }
        if (CurrentMode == OBJECT_MODE.NoiseDirectory)
        {
            List<KeyframeData<string>> list = PartFile.GetInstance().KeyFrames.NoiseFileKeyframes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].FrameNum == SelectedFrame)
                { list.RemoveAt(i); break; }
            }
        }

        UpdateKeyframes();
        RefreshObjectState();
    }

    public void OnPlayButtonPressed()
    {
        if (CurrentMode == OBJECT_MODE.Render)
            return;
        isPlayingPreview = !isPlayingPreview;
    }

    void UpdateFrameNumber()
    {
        if (CurrentMode == OBJECT_MODE.Render)
            return;
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
        if(isPlayingPreview)
        {
            playingPreviewTimer += Time.deltaTime;
            if(playingPreviewTimer > 0.05f)
            {
                playingPreviewTimer -= 0.05f;
                changedPos = true;
                SelectedFrame++;
                if (SelectedFrame >= PartFile.GetInstance().FrameCount)
                    SelectedFrame = 0;
            }
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

    public void UpdateDropdown(bool returningFromRender = false)
    {
        if (returningFromRender == false && CurrentMode == OBJECT_MODE.Render)
            return;
        
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
        if (id == 9)
            DropdownLightingDetected4();
        if (id == 10)
            DropdownFresnel2Selected();
        if (id == 11)
            DropdownGlowSelected();
        if (id == 12)
            DropdownNoiseTextureSelected();

        for (int i = 0; i < EditingWindows.Length; i++)
        {
            EditingWindows[i].SetActive(i == id);
        }
        UpdateKeyframes();
        RefreshObjectState();
    }

    void DropdownNoiseTextureSelected()
    {
        CurrentMode = OBJECT_MODE.NoiseDirectory;
        FrameTypeText.text = "Keyframes: Noise Directory";
    }

    /// <summary>
    /// Set to no editing window and render mode when you click the render button
    /// </summary>
    public void OnRenderClicked()
    {
        isPlayingPreview = false;  //Turn off play while we're rendering.
        CurrentMode = OBJECT_MODE.Render;
        for (int i = 0; i < EditingWindows.Length; i++)
        {
            EditingWindows[i].SetActive(false);
        }
        RefreshObjectState();
    }

    /// <summary>
    /// Set when you 'x' out of render mode
    /// </summary>
    public void OnRenderDone()  
    {
        UpdateDropdown(true);
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
    public void DropdownFresnel2Selected()
    {
        CurrentMode = OBJECT_MODE.Fresnel2;
        FrameTypeText.text = "Keyframes: Inner Fresnel";
    }

    public void DropdownVertexSelected()
    {
        CurrentMode = OBJECT_MODE.Vertex;
        FrameTypeText.text = "Keyframes: Shape";
    }

    public void DropdownLightingDetected1()
    {
        CurrentMode = OBJECT_MODE.Lighting1;
        FrameTypeText.text = "Keyframes: Dir Lighting Direction";
    }
    public void DropdownLightingDetected2()
    {
        CurrentMode = OBJECT_MODE.Lighting2;
        FrameTypeText.text = "Keyframes: Dir Lighting Intensity";
    }
    public void DropdownLightingDetected3()
    {
        CurrentMode = OBJECT_MODE.Lighting3;
        FrameTypeText.text = "Keyframes: Dir Light Color";
    }

    public void DropdownLightingDetected4()
    {
        CurrentMode = OBJECT_MODE.Lighting4;
        FrameTypeText.text = "Keyframes: Ambient Light Color";
    }

    public void DropdownGlowSelected()
    {
        CurrentMode = OBJECT_MODE.Glow;
        FrameTypeText.text = "Keyframes: Glowing";
    }
}
