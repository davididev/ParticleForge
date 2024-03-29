using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static float RotateSnap = 15f;
    public enum OffsestDirection { X, Y, Z};
    public OffsestDirection thisObjectOffset;
    public Image rend;

    public static bool IsDragging = false;

    private Vector2 startMousePosition;
    private float lastDegree = 0f;
    private float startingDegree = 0f;
    public static Vector3 NewWorldPosition = Vector3.zero;
    public static Vector3 OffsetStep = Vector3.zero;  //This should be obtained by a panel and then set back to zero once it has been obtained
    RectTransform parent;

    // Start is called before the first frame update
    void OnEnable()
    {
        parent = transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        rend.color = Color.white;
    }


    public void OnPointerExit(PointerEventData pointerEventData)
    {
        rend.color = Color.grey;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startMousePosition = eventData.position;
        lastDegree = 0f;
        startingDegree = Mathf.Atan2(startMousePosition.y, startMousePosition.x) * Mathf.Rad2Deg;
        IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = startMousePosition - eventData.position;
        
        //Debug.Log("Offset of " + gameObject.name + ":" + offset);

        if (thisObjectOffset == OffsestDirection.X)
        {
            if (offset.x > 20f)
            {
                OffsetStep.y += RotateSnap;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
            if (offset.x < -20f)
            {
                OffsetStep.y -= RotateSnap;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
        }
        if (thisObjectOffset == OffsestDirection.Y)
        {
            if (offset.y > 20f)
            {
                OffsetStep.x += RotateSnap;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
            if (offset.y < -20f)
            {
                OffsetStep.x -= RotateSnap;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
        }

        if(thisObjectOffset == OffsestDirection.Z)
        {
            float newAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            float dif = Mathf.DeltaAngle(startingDegree, newAngle);
            //Debug.Log("Difference of angles: " + dif);
            if (dif > 15f)
            {
                if (offset.magnitude > 40f) //Lower the sensitiviy- only rotate if the offset is far from the center
                    OffsetStep.z += RotateSnap;
                startingDegree += RotateSnap;
            }
            if (dif < -15f)
            {
                if (offset.magnitude > 40f) //Lower the sensitiviy- only rotate if the offset is far from the center
                    OffsetStep.z -= RotateSnap;
                startingDegree -= RotateSnap;
            }
        }
    }

    public void SetArrowsByWorldPosition()
    {
        //Vector3 localPos = parent.InverseTransformVector();
        parent.position = Camera.main.WorldToScreenPoint(TransformScaleArrow.NewWorldPosition);
    }
}
