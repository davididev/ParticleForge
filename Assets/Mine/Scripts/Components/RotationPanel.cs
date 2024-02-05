using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public StartingShapeHolder refToShape;
    public GameObject transformRotate;
    public TMPro.TMP_InputField[] RotationEulerText;
    void OnEnable()
    {
        transformRotate.SetActive(true);
    }

    void OnDisable()
    {
        transformRotate.SetActive(false);
    }



    void RefreshUI()  //Refresh the UI when a keyframe is added or timeline is moving
    {
        List<KeyframeData<Vector3>> tempData = PartFile.GetInstance().KeyFrames.RotationKeyframes;
        Vector3 rot1 = Vector3.zero;
        Vector3 rot2 = Vector3.zero;
        float lerp = 0f;
        KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData);
        Vector3 currentFrame = Vector3.Lerp(rot1, rot2, lerp);

        RotationEulerText[0].text = currentFrame.x.ToString();
        RotationEulerText[1].text = currentFrame.y.ToString();
        RotationEulerText[2].text = currentFrame.z.ToString();
    }

    //We're moving this to StartingShapeHolder
    /*
    void RefreshTimeline()
    {
        List<KeyframeData<Vector3>> tempData = PartFile.GetInstance().KeyFrames.RotationKeyframes;
        Vector3 rot1 = Vector3.zero;
        Vector3 rot2 = Vector3.zero;
        float lerp = 0f;
        KeyframeData<Vector3>.GetLerpAmount(KeyframeMainWindow.SelectedFrame, out rot1, out rot2, out lerp, tempData);
        Vector3 currentFrame = Vector3.Lerp(rot1, rot2, lerp);
        
        RotationEulerText[0].text = currentFrame.x.ToString();
        RotationEulerText[1].text = currentFrame.y.ToString();
        RotationEulerText[2].text = currentFrame.z.ToString();

        if (refToShape.CurrentShape != null)
            refToShape.CurrentShape.transform.localEulerAngles = currentFrame;
        
    }
    */

    public void PreviewEuler()
    {
        if (KeyframeMainWindow.PreviewEffect == true)
            return;
        StartCoroutine(PreviewEulerRoutine());
    }

    IEnumerator PreviewEulerRoutine()
    {
        KeyframeMainWindow.PreviewEffect = true;
        Vector3 prevAngles = refToShape.CurrentShape.transform.localEulerAngles;
        Vector3 v = Vector3.zero;
        v.x = float.Parse(RotationEulerText[0].text);
        v.y = float.Parse(RotationEulerText[1].text);
        v.z = float.Parse(RotationEulerText[2].text);
        refToShape.CurrentShape.transform.localEulerAngles = v;
        yield return new WaitForSeconds(0.5f);

        refToShape.CurrentShape.transform.localEulerAngles = prevAngles;
        KeyframeMainWindow.PreviewEffect = false;
    }

    public void AddKeyframe()
    {
        Vector3 v = Vector3.zero;
        v.x = float.Parse(RotationEulerText[0].text);
        v.y = float.Parse(RotationEulerText[1].text);
        v.z = float.Parse(RotationEulerText[2].text);
        PartFile.GetInstance().KeyFrames.AddKeyframeRotation(KeyframeMainWindow.SelectedFrame, v);
    }

    // Update is called once per frame
    void Update()
    {
        if(TransformRotate.OffsetStep != Vector3.zero)  //Rotate circles have been used, rotate the object
        {
            refToShape.CurrentShape.transform.Rotate(TransformRotate.OffsetStep, Space.World);
            TransformRotate.OffsetStep = Vector3.zero;
            Vector3 newEulers = refToShape.CurrentShape.transform.localEulerAngles;

            RotationEulerText[0].text = newEulers.x.ToString();
            RotationEulerText[1].text = newEulers.y.ToString();
            RotationEulerText[2].text = newEulers.z.ToString();
        }
    }
}
