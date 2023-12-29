using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SkywardRay.FileBrowser {
	public class SfbPromtNewFolder: SfbPromt {
		public Button buttonAction;
		public Button buttonCancel;
		public SfbInputField inputField;

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
			inputField.Submit();
			ListenerCancel();
		}

		private void ListenerCancel () {
			Close();
		}

		public override void Init (SfbInternal fileBrowser) {
			this.fileBrowser = fileBrowser;
			SetListeners();

			// Init because the start function won't be called until next frame if it's just been instantiated
			inputField.Init();
			var text = inputField.GetText();

			Stopwatch sw = Stopwatch.StartNew();
			int count = 1;
			while (fileBrowser.DirectoryExistsInCurrentDirectory(text)) {
				if (sw.ElapsedMilliseconds > 500) {
					text = inputField.GetText();
					Debug.Log("Checking existing directories took too long, break.");
					break;
				}

				text = inputField.GetText() + " (" + count + ")";
				count++;
			}

			inputField.SetText(text);
		}
	}
}