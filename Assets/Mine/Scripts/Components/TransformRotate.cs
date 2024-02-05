using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum OffsestDirection { X, Y, Z};
    public OffsestDirection thisObjectOffset;
    public Image rend;

    private Vector2 startMousePosition;
    public static Vector3 OffsetStep = Vector3.zero;  //This should be obtained by a panel and then set back to zero once it has been obtained

    // Start is called before the first frame update
    void Start()
    {
        
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
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = startMousePosition - eventData.position;

        Debug.Log("Offset of " + gameObject.name + ":" + offset);

        if (thisObjectOffset == OffsestDirection.X)
        {
            if (offset.x > 20f)
            {
                OffsetStep.y += 15f;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
            if (offset.x < -20f)
            {
                OffsetStep.y -= 15f;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
        }
        if (thisObjectOffset == OffsestDirection.Y)
        {
            if (offset.y > 20f)
            {
                OffsetStep.x += 15f;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
            if (offset.y < -20f)
            {
                OffsetStep.x -= 15f;
                startMousePosition = eventData.position;  //Reset the offset for the next rotate step
            }
        }
    }
}
