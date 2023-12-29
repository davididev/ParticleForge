using UnityEngine;
using UnityEngine.EventSystems;

namespace SkywardRay.FileBrowser {
	public class SfbDisabledWindowOverlay : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler {
		private SfbInternal fileBrowser;

		private void Start () {
			fileBrowser = GetComponentInParent<SfbWindow>().fileBrowser;
		}

		public void OnPointerDown (PointerEventData eventData) {}

		public void OnPointerUp (PointerEventData eventData) {}

		public void OnDrag (PointerEventData eventData) {}

		public void OnPointerClick (PointerEventData eventData) {
			fileBrowser.ListenerDisabledWindowPanel();
		}
	}
}