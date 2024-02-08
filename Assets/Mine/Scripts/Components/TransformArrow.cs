using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformArrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum OffsestDirection { UP, RIGHT };
    public OffsestDirection thisObjectOffset;
    public Image rend;

    private Vector3 startingRootPosition;
    public static bool IsDragging = false;
    private Vector2 startMousePosition;
    private Vector2 startTransformPosition { get; set; }

    public static Vector3 NewWorldPosition = Vector3.zero;
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
        Vector3 worldPos = parent.TransformVector(parent.position);  //Get screen position of anchored position
        NewWorldPosition = Camera.main.ScreenToWorldPoint(worldPos, Camera.MonoOrStereoscopicEye.Mono);
        NewWorldPosition.z = 0f;
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
        Vector2 anchoredPosition = startTransformPosition + offset;
        if (anchoredPosition.x < 50f)
            anchoredPosition.x = 50f;
        if (anchoredPosition.x > 750f)
            anchoredPosition.x = 750f;
        if (anchoredPosition.y > 50f)
            anchoredPosition.y = 50f;
        if (anchoredPosition.y < -600f)
            anchoredPosition.y = -600f;

        
        parent.anchoredPosition = anchoredPosition;
        /*
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, parent.anchoredPosition, Camera.main, out worldPos))
        {
            Debug.Log("World Pos: " + worldPos);
            NewWorldPosition = worldPos;
            NewWorldPosition.z = 0f;
        }
        */

    }

    public void SetArrowsByWorldPosition()
    {
        //Vector3 localPos = parent.InverseTransformVector();
        parent.position = Camera.main.WorldToScreenPoint(TransformArrow.NewWorldPosition);
    }




}
