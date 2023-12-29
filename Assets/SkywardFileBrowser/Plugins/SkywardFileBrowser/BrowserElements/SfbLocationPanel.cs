using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbLocationPanel : SfbPanel {
		public SfbEntry prefabFileEntry;
		public SfbEntry prefabFolderEntry;
		public SfbEntry prefabLogicalDriveEntry;
		public SfbEntry prefabRecentHeader;
		public SfbEntry prefabFavoriteHeader;
		public string RecentHeaderText = "";
		public string FavortieHeaderText = "";

		public void Repopulate (List<SfbSavedLocationEntry> entries, bool keepScrollPosition) {
			lastScrollPosition = scrollRect.verticalNormalizedPosition;

			if (entryPrefabs.Count == 0) {
				return;
			}
			if (!keepScrollPosition) {
				lastScrollPosition = 1;
			}
			for (int i = 0; i < content.childCount; i++) {
				Destroy(content.GetChild(i).gameObject);
			}

			CreateWrappers(entries);
			StartShowOnScreenEntries(lastScrollPosition);

			StartCoroutine(UpdateScrollView());
		}

		private void CreateWrappers (List<SfbSavedLocationEntry> entries) {
			wrappers.Clear();

			var recent = entries.Where(a => a.locationType == SfbSavedLocationType.Recent).OrderByDescending(a => a.savedDate).ToList();
			if (recent.Any()) {
				AddHeader(SfbEntryType.HeaderRecent);

				foreach (SfbSavedLocationEntry entry in recent) {
					CreateWrapper(entry);
				}
			}
			var favorite = entries.Where(a => a.locationType == SfbSavedLocationType.Favorite).ToList();
			if (favorite.Any()) {
				AddHeader(SfbEntryType.HeaderFavorite);

				foreach (SfbSavedLocationEntry entry in favorite) {
					CreateWrapper(entry);
				}
			}
		}

		private void CreateWrapper (SfbSavedLocationEntry entry) {
			var wrapper = SfbEntryWrapper.CreateEmpty((entryPrefabs.First(a => a.type == entry.type.Convert()).transform as RectTransform).rect.height);
			wrapper.fileSystemEntry = entry;
			wrapper.rectTransform.SetParent(content, false);
			wrapper.parent = this;
			wrappers.Add(wrapper);
		}

		private void AddHeader (SfbEntryType type) {
			var go = Instantiate(entryPrefabs.First(a => a.type == type).gameObject);
			var wrapper = SfbEntryWrapper.CreateEmpty((go.transform as RectTransform).rect.height);
			wrapper.transform.SetParent(content, false);
			wrapper.parent = this;
			wrapper.interactable = false;
			wrappers.Add(wrapper);

			string text = type == SfbEntryType.HeaderRecent ? RecentHeaderText : FavortieHeaderText;
			if (text == "") {
				text = type.ToString().Replace("Header", "") + ":";
			}
			
			go.name = text;
			go.transform.SetParent(wrapper.transform, false);
			go.GetComponentInChildren<Text>().text = text;

			var entry = go.GetComponent<SfbEntry>();
			entry.wrapper = wrapper;
			wrapper.BrowserEntry = entry;
		}

		public override void Init (SfbInternal fileBrowser) {
			if (prefabFileEntry == null || prefabFolderEntry == null || prefabLogicalDriveEntry == null || prefabRecentHeader == null || prefabFavoriteHeader == null) {
				Debug.LogError("Prefabs not set correctly in inspector");
				return;
			}

			entryPrefabs.Add(prefabFileEntry);
			entryPrefabs.Add(prefabFolderEntry);
			entryPrefabs.Add(prefabLogicalDriveEntry);
			entryPrefabs.Add(prefabRecentHeader);
			entryPrefabs.Add(prefabFavoriteHeader);
			base.Init(fileBrowser);
		}
	}
}