using UnityEngine;
using UnityEngine.EventSystems;

namespace SkywardRay.FileBrowser {
    public class SfbResizeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	    public SfbResizeSide resizeSide = SfbResizeSide.None;

        private SfbResizeable resizeable;
        private Vector2 oldpos = Vector2.zero;

        // Use this for initialization
        private void Start () {
	        resizeable = GetComponentInParent<SfbResizeable>();
        }

		/// <summary>
		/// Called by Unity, used for resizing the window
		/// </summary>
        public void OnPointerDown (PointerEventData eventData) {
            oldpos = eventData.position;
            resizeable.ButtonResize(resizeSide);
        }

		/// <summary>
		/// Called by Unity, used for resizing the window
		/// </summary>
        public void OnPointerUp (PointerEventData eventData) {
            resizeable.EndResize();
        }

		/// <summary>
		/// Called by Unity, used for resizing the window
		/// </summary>
        public void OnDrag (PointerEventData eventData) {
            Vector2 screenPoint;
            Vector2 oldPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(resizeable.transform as RectTransform, eventData.position, eventData.pressEventCamera, out screenPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(resizeable.transform as RectTransform, oldpos, eventData.pressEventCamera, out oldPoint);

            resizeable.Resize(screenPoint, oldPoint);
            oldpos = eventData.position;
        }
    }
}