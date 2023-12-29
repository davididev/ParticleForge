using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbFilePanel : SfbPanel {
		public SfbEntry prefabFileEntry;
		public SfbEntry prefabFolderEntry;
		public SfbEntry prefabLogicalDriveEntry;
		
		public override void Init (SfbInternal fileBrowser) {
			if (prefabFileEntry == null || prefabFolderEntry == null || prefabLogicalDriveEntry == null) {
				Debug.LogError("Prefabs not set correctly in inspector");
				return;
			}

			entryPrefabs.Add(prefabFileEntry);
			entryPrefabs.Add(prefabFolderEntry);
			entryPrefabs.Add(prefabLogicalDriveEntry);
			base.Init(fileBrowser);
		}

		public new void Repopulate (IEnumerable<SfbFileSystemEntry> entries, bool keepScrollPosition) {
			base.Repopulate(entries, keepScrollPosition);
		}
	}
}