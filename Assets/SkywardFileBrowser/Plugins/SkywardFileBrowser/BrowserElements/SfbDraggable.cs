using UnityEngine;
using UnityEngine.EventSystems;

namespace SkywardRay.FileBrowser {
	public class SfbDraggable : MonoBehaviour, IDragHandler, IPointerDownHandler {
		private Vector2 oldpos = Vector2.zero;
		private RectTransform rectTransform;
		private RectTransform canvasRectTransform;

		public void Start () {
			rectTransform = transform as RectTransform;
			canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
		}

		public void OnDrag (PointerEventData data) {
			if (rectTransform == null) {
				return;
			}

			Vector2 screenPoint;
			Vector2 oldPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out screenPoint);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, oldpos, data.pressEventCamera, out oldPoint);

			Vector2 delta = screenPoint - oldPoint;

			Vector2 canvasScreenPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, data.position, data.pressEventCamera, out canvasScreenPoint);

			if (canvasRectTransform.rect.Contains(canvasScreenPoint, true)) {
				rectTransform.anchoredPosition = rectTransform.anchoredPosition + delta;
			}

			oldpos = data.position;
		}

		public void OnPointerDown (PointerEventData data) {
			oldpos = data.position;
		}
	}
}