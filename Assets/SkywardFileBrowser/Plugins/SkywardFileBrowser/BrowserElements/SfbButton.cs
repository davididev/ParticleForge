using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	[RequireComponent(typeof(Button))]
	public class SfbButton : MonoBehaviour, IDragHandler {
		[SerializeField]
		public SfbButtonAction action = SfbButtonAction.NOT_SET;

		public void SetListeners () {
			SfbInternal fileBrowser = GetComponentInParent<SfbWindow>().fileBrowser;
			if (fileBrowser == null) {
				return;
			}
			fileBrowser.SetButtonListeners(GetComponent<Button>(), action);
		}

		// This is here to stop the browser window from being dragged when dragging on a button
		public void OnDrag (PointerEventData eventData) {}
	}
}