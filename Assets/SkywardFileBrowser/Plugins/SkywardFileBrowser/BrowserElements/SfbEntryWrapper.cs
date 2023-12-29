using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbEntryWrapper : MonoBehaviour {
		public SfbFileSystemEntry fileSystemEntry;
		public RectTransform rectTransform;
		public SfbPanel parent;
		public bool interactable = true;

		private bool selected = false;
		private bool pressed = false;
		private SfbEntry browserEntry;

		public SfbEntry BrowserEntry {
			get {
				return browserEntry;
			}
			set {
				browserEntry = value;
				browserEntry.Selected = selected;
				browserEntry.Pressed = pressed;
			}
		}

		public bool EntryActive {
			get {
				return BrowserEntry != null && BrowserEntry.gameObject.activeInHierarchy;
			}
			set {
				if (BrowserEntry != null && BrowserEntry.gameObject.activeInHierarchy != value) {
					BrowserEntry.gameObject.SetActive(value);
				} 
			}
		}

		public bool Selected {
			get {
				return selected;
			}
			set {
				if (interactable) {
					selected = value;

					if (BrowserEntry != null) {
						BrowserEntry.Selected = selected;
					}
				}
			}
		}

		public bool Pressed {
			get {
				return pressed;
			}
			set {
				if (interactable) {
					pressed = value;
					if (BrowserEntry != null) {
						BrowserEntry.Pressed = pressed;
					}
				}
			}
		}

		public static SfbEntryWrapper CreateEmpty (float height) {
			GameObject go = new GameObject("Entry wrapper", typeof(RectTransform));

			var layoutElement = go.AddComponent<LayoutElement>();
			layoutElement.flexibleWidth = 1;
			layoutElement.minHeight = height;

			var wrapper = go.AddComponent<SfbEntryWrapper>();
			wrapper.rectTransform = go.transform as RectTransform;

			return wrapper;
		}
	}
}