using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public abstract class SfbPromt : MonoBehaviour, SfbIElement {
		protected SfbInternal fileBrowser;
		
		protected virtual void SetListeners () {}

		public abstract void Init (SfbInternal fileBrowser);

		public void SetText (string text) {
			var tc = GetComponentInChildren<Text>();
			if (tc == null) {
				return;
			}

			tc.text = text;
		}

		public void Close () {
			fileBrowser.ClosingOpenPromt(this);
			Destroy(gameObject);
		}

		public void SetFocus () {
			throw new System.NotImplementedException();
		}

		public void ReceiveMessage (string message) {
			throw new System.NotImplementedException();
		}
	}
}