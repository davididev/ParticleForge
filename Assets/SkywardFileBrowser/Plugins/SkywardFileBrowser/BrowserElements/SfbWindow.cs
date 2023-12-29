using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SkywardRay.FileBrowser {
	public class SfbWindow : MonoBehaviour {
		public SfbInternal fileBrowser;

		private void Start () {
			SfbButton[] buttons = GetComponentsInChildren<SfbButton>(true);
			foreach (SfbButton button in buttons) {
				button.SetListeners();
			}
		}

		public void Init (SfbInternal fileBrowser) {
			this.fileBrowser = fileBrowser;

			var children = GetComponentsInChildren<SfbIElement>();
			foreach (var child in children) {
				child.Init(fileBrowser);
			}
		}
	}
}