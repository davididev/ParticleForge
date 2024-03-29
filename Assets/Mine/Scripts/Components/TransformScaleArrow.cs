using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   

public class TransformScaleArrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static float ScaleSnap = 0.1f;
    public enum OffsestDirection { UP, RIGHT };
    public OffsestDirection thisObjectOffset;
    public Image rend;

    public static bool IsDragging = false;
    private Vector2 startMousePosition;
    private Vector2 startTransformPosition { get; set; }
    public static Vector3 NewWorldPosition = Vector3.zero;
    public static Vector3 NewScale = Vector3.one;
    RectTransform parent;
    // Start is called before the first frame update
    void OnEnable()
    {
        rend.color = Color.grey;
        startTransformPosition = transform.parent.GetComponent<RectTransform>().anchoredPosition;
        parent = transform.parent.GetComponent<RectTransform>();
    }

    void OnDisable()  //Object was hidden from view, reset the offset
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
        IsDragging = true;
        startMousePosition = eventData.position;
        startTransformPosition = transform.parent.GetComponent<RectTransform>().anchoredPosition;
        transform.parent.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset = eventData.position - startMousePosition;
        if (thisObjectOffset == OffsestDirection.UP)
            offset.x = 0f;
        if (thisObjectOffset == OffsestDirection.RIGHT)
            offset.y = 0f;

        Vector3 rel = offset;
        //Debug.Log("Rel: " + rel);
        
        if (rel.x < -20f)  //Scale down X
        {
            NewScale.x -= ScaleSnap;
            NewScale.x = Mathf.Clamp(NewScale.x, 0.1f, 6f);
            startMousePosition = eventData.position;
        }
        if (rel.x > 50f)  //Scale up X
        {
            NewScale.x += ScaleSnap;
            NewScale.x = Mathf.Clamp(NewScale.x, 0.1f, 6f);
            startMousePosition = eventData.position;
        }
        if (rel.y < -20f)  //Scale down Y
        {
            NewScale.y -= ScaleSnap;
            NewScale.y = Mathf.Clamp(NewScale.y, 0.1f, 6f);
            startMousePosition = eventData.position;
        }
        if (rel.y > 50f)  //Scale up Y
        {
            NewScale.y += ScaleSnap;
            NewScale.y = Mathf.Clamp(NewScale.y, 0.1f, 6f);
            startMousePosition = eventData.position;
        }

        Debug.Log(Time.time + ": New Scale: " + NewScale.ToString());

        //parent.anchoredPositi on = anchoredPosition;
        /*
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, parent.anchoredPosition, Camera.main, out worldPos))
        {
            Debug.Log("World Pos: " + worldPos);
            NewWorldPosition = worldPos;
            NewWorldPosition.z = 0f;
        }
        */

    }




}
