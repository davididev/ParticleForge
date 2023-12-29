using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbDropMenu : MonoBehaviour {
		public SfbDropMenuItem prefabItem;
		public Transform content;
		public SfbDropMenuType type;
		public float maxHeight = 240f;

		private SfbInternal fileBrowser;
		private List<SfbDropMenuItem> items = new List<SfbDropMenuItem>();

		public void Repopulate (IEnumerable<string> input) {
			if (type == SfbDropMenuType.NOT_SET) {
				Debug.LogError("DropMenu type not set.");
				return;
			}
			if (fileBrowser == null) {
				fileBrowser = GetComponentInParent<SfbWindow>().fileBrowser;
			}
			if (prefabItem == null || prefabItem.GetComponent<SfbDropMenuItem>() == null) {
				Debug.LogError("DropMenu item prefab not set or missing BrowserDropMenuItem component");
				return;
			}

			for (int i = 0; i < content.childCount; i++) {
				Destroy(content.GetChild(i).gameObject);
			}
			items.Clear();

			foreach (var item in input) {
				AddItem(item);
			}

			float height = items.Count() * prefabItem.GetComponent<RectTransform>().sizeDelta.y;
			if (height > maxHeight) {
				height = maxHeight;
			}
			else {
				
			}
			RectTransform rt = GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
		}

		private void AddItem (string input) {
			GameObject go = Instantiate(prefabItem).gameObject;
			go.name = input;
			go.transform.SetParent(content, false);
			go.GetComponent<SfbDropMenuItem>().text = input;

			go.GetComponentInChildren<Text>().text = input;

			items.Add(go.GetComponent<SfbDropMenuItem>());
		}

		public void ClickItem (SfbDropMenuItem item) {
			fileBrowser.ListenerDropMenu(type, item.text);
			gameObject.SetActive(false);
		}
	}
}