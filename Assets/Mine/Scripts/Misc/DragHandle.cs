using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ui
{
    public class DragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform Parent;
        //private RectTransform RectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();
        
        private Vector2 StartMousePosition { get; set; }
        private Vector2 StartTransformPosition { get; set; }
        private int? DraggingPointerId { get; set; }
        private bool IsDragging => DraggingPointerId.HasValue;

        private void OnValidate()
        {
            //RectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        void OnEnable()
        {
            //Added this.  When you open a new panel, set it on top
            Parent.SetAsLastSibling();
        }

        private void Awake()
        {
            OnValidate();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //if (IsDragging)
            //{
              //  return;
            //}

            Parent.SetAsLastSibling();

            DraggingPointerId = eventData.pointerId;
            StartMousePosition = eventData.position;
            StartTransformPosition = Parent.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }
            
            
            Vector2 offset = eventData.position - StartMousePosition;
            Vector2 anchoredPosition = StartTransformPosition + offset;
            if (anchoredPosition.x < -400f)
                anchoredPosition.x = -400f;
            if (anchoredPosition.x > 400f)
                anchoredPosition.x = 400f;
            if (anchoredPosition.y < -375f)
                anchoredPosition.y = -375f;
            if (anchoredPosition.y > 375f)
                anchoredPosition.y = 375f;
            //for (int i = 0; i < 2; i++)
            //{
            //float halfSize = 0.5f * (parentRect.size[i] - rect.size[i]);
            //  anchoredPosition[i] = Mathf.Clamp(anchoredPosition[i], -halfSize, halfSize);
            //}

            Parent.anchoredPosition = anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging || DraggingPointerId.GetValueOrDefault() == eventData.pointerId)
            {
                return;
            }

            DraggingPointerId = null;
        }
    }
}
