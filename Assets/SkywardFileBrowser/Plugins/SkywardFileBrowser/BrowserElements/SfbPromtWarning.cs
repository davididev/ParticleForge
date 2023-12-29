using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbPromtWarning : SfbPromt {
		public Button buttonConfirm;

		public override void Init (SfbInternal fileBrowser) {
			this.fileBrowser = fileBrowser;
			SetListeners();
		}

		protected override void SetListeners () {
			if (buttonConfirm != null) {
				buttonConfirm.onClick.AddListener(Close);
			}
			else {
				Debug.LogWarning("buttonConfirm is not set in the inspector");
			}
		}
	}
}