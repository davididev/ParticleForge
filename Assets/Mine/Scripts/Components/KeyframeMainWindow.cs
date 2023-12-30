using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyframeMainWindow : MonoBehaviour
{
    public RectTransform CursorPosition;
    public RectTransform BackgroundOfTimeline;
    public GameObject MarkerPrefab;
    private GameObject[] MarkerGameObjects;
    public TMPro.TextMeshProUGUI FrameNumberText, FrameTypeText;
    private int SelectedFrame = 1;
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
    /// When a new frame is selected, update rotation / shape / etc
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
    }
}
