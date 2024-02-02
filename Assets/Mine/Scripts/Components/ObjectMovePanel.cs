using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectMovePanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject transformArrows;
    public StartingShapeHolder refToShape;
    public TMPro.TMP_InputField[] PositionTexts;
    void OnEnable()
    {
        transformArrows.SetActive(true);
        PositionTexts[0].text = TransformArrow.NewWorldPosition.x.ToString("0.00");
        PositionTexts[1].text = TransformArrow.NewWorldPosition.y.ToString("0.00");
    }

    void OnDisable()
    {
        transformArrows.SetActive(false);
    }

    public void OnUpdateTextBoxes()
    {
        float x = 0f;
        float y = 0f;
        if (float.TryParse(PositionTexts[0].text, out x) == false)
            return;
        if (float.TryParse(PositionTexts[1].text, out y) == false)
            return;

        TransformArrow.NewWorldPosition = new Vector3(x, y, 0f);
        transformArrows.transform.GetChild(0).GetComponent<TransformArrow>().SetArrowsByWorldPosition();
    }

    Vector3 LastWorldPosition = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if (TransformArrow.NewWorldPosition != LastWorldPosition)  //Don't update the text box position if it didn't move
        {
            LastWorldPosition = TransformArrow.NewWorldPosition;
            refToShape.transform.position = TransformArrow.NewWorldPosition;
            PositionTexts[0].text = TransformArrow.NewWorldPosition.x.ToString("0.00");
            PositionTexts[1].text = TransformArrow.NewWorldPosition.y.ToString("0.00");
            transformArrows.transform.GetChild(0).GetComponent<TransformArrow>().SetArrowsByWorldPosition();
        }
    }

    public void AddKeyframe()
    {
        float x = 0f;
        float y = 0f;
        if (float.TryParse(PositionTexts[0].text, out x) == false)
            return;
        if (float.TryParse(PositionTexts[1].text, out y) == false)
            return;

        PartFile.GetInstance().KeyFrames.AddKeyframePosition(KeyframeMainWindow.SelectedFrame, new Vector2(x, y));
    }
}
