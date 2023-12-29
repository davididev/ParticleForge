using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	[RequireComponent(typeof(Button))]
	public class SfbDropMenuItem : MonoBehaviour {
		public string text = "";

		private SfbDropMenu parent;

		void Start () {
			parent = GetComponentInParent<SfbDropMenu>();
			GetComponent<Button>().onClick.AddListener(Click);
		}

		void Click () {
			parent.ClickItem(this);
		}
	}
}