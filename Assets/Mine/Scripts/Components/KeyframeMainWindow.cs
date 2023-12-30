using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyframeMainWindow : MonoBehaviour
{
    public RectTransform CursorPosition;
    public RectTransform BackgroundOfTimeline;
    public GameObject MarkerPrefab;
    private GameObject[] MarkerGameObjects;
    public TMPro.TMP_Dropdown DropdownObjectMode;
    public TMPro.TextMeshProUGUI FrameNumberText, FrameTypeText;
    private int SelectedFrame = 1;
    public enum OBJECT_MODE { Vertex, Rotation, Position, NoiseOffset, SetColor, Fresnel};
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
    }

    /// <summary>
    /// When a new frame is selected, or when a keyframe is updated (to update rotation, shape, color, etc)
    /// </summary>
    public void RefreshObjectState()
    {
        //Add functionality here 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrameNumber();
    }

    void UpdateFrameNumber()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectedFrame++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectedFrame--;
        }
        SelectedFrame = Mathf.Clamp(SelectedFrame, 1, PartFile.GetInstance().FrameCount);

        Vector3 localPos = CursorPosition.localPosition;
        localPos.x = (SelectedFrame - 1) * 12.0f;
        CursorPosition.localPosition = localPos;

        FrameNumberText.text = "F: " + SelectedFrame + " / " + PartFile.GetInstance().FrameCount;
        RefreshObjectState();
    }

    public void UpdateDropdown()
    {
        int id = DropdownObjectMode.value;
        if (id == 0)
            DropdownRotationSelected();
        if (id == 1)
            DropdownPositionSelected();
        if (id == 2)
            DropdownNoiseOffsetSelected();
        if (id == 3)
            DropdownColorSelected();
        if (id == 4)
            DropdownFresnelSelected();
        if (id == 5)
            DropdownVertexSelected();
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
}
