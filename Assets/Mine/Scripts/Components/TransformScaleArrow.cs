using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformScaleArrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum OffsestDirection { UP, RIGHT };
    public OffsestDirection thisObjectOffset;
    public Image rend;

    private bool IsDragging = false;
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
        Vector2 anchoredPosition = startTransformPosition + offset;
        if (anchoredPosition.x < 50f)
            anchoredPosition.x = 50f;
        if (anchoredPosition.x > 750f)
            anchoredPosition.x = 750f;
        if (anchoredPosition.y > 50f)
            anchoredPosition.y = 50f;
        if (anchoredPosition.y < -600f)
            anchoredPosition.y = -600f;

        Vector3 rel = offset - startTransformPosition;
        if (rel.x < 0f)  //Scale down X
        {
            float scaleX = Mathf.Round(rel.x / -50f / 20f) * 20f;
            scaleX = Mathf.Clamp(scaleX, 0.05f, 1f);
            NewScale.x = scaleX;
        }
        if (rel.x > 0f)  //Scale up X
        {
            float scaleX = 1f + Mathf.Round(rel.x / -50f / 20f) * 20f;
            scaleX = Mathf.Clamp(scaleX, 1f, 6f);
            NewScale.x = scaleX;
        }
        if (rel.y < 0f)  //Scale down Y
        {
            float scaleY = Mathf.Round(rel.y / -50f / 20f) * 20f;
            scaleY = Mathf.Clamp(scaleY, 0.05f, 1f);
            NewScale.y = scaleY;
        }
        if (rel.y > 0f)  //Scale up Y
        {
            float scaleY = 1f + Mathf.Round(rel.y / -50f / 20f) * 20f;
            scaleY = Mathf.Clamp(scaleY, 1f, 6f);
            NewScale.y = scaleY;
        }

        //parent.anchoredPosition = anchoredPosition;
        Vector3 worldPos;
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
        parent.position = Camera.main.WorldToScreenPoint(TransformScaleArrow.NewWorldPosition);
    }




}
