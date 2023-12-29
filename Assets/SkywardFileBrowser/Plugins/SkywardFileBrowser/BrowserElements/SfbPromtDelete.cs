using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbPromtDelete : SfbPromt {
		public Button buttonAction;
		public Button buttonCancel;

		protected override void SetListeners () {
			if (buttonAction != null) {
				buttonAction.onClick.AddListener(ListenerConfirm);
			}
			else {
				Debug.LogWarning("buttonAction is not set in the inspector");
			}
			if (buttonCancel != null) {
				buttonCancel.onClick.AddListener(ListenerCancel);
			}
			else {
				Debug.LogWarning("buttonCancel is not set in the inspector");
			}
		}

		public void AddActionButtonListener (UnityAction action) {
			if (buttonAction == null) {
				return;
			}

			buttonAction.onClick.AddListener(action);
		}

		public void AddCancelButtonListener (UnityAction action) {
			if (buttonCancel == null) {
				return;
			}

			buttonCancel.onClick.AddListener(action);
		}

		private void ListenerConfirm () {
			fileBrowser.ListenerDeleteSelectionConfirm();
			ListenerCancel();
		}

		private void ListenerCancel () {
			Close();
		}

		public override void Init (SfbInternal fileBrowser) {
			this.fileBrowser = fileBrowser;
			SetListeners();
		}
	}
}