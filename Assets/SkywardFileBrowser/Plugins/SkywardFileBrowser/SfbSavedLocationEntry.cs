using System;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbSavedLocationEntry : SfbFileSystemEntry {
		public SfbSavedLocationType locationType;
		public DateTime savedDate;

		public SfbSavedLocationEntry (SfbSavedLocationType locationType, string path, bool hidden, SfbFileSystemEntryType type) : base(path, hidden, type) {
			this.locationType = locationType;
			savedDate = DateTime.Now;
		}

		private SfbSavedLocationEntry (SfbSavedLocationType locationType, DateTime savedDate, string path, bool hidden, SfbFileSystemEntryType type) : base(path, hidden, type) {
			this.locationType = locationType;
			this.savedDate = savedDate;
		}

		public string FormatForSave () {
			string val = "SavedLocationEntry {\n";

			val += locationType + ",\n";
			val += path + ",\n";
			val += hidden + ",\n";
			val += type + ",\n";
			val += savedDate + "\n";

			return val + "}\n";
		}

		public static SfbSavedLocationEntry FromFileSystemEntry (SfbSavedLocationType locationType, SfbFileSystemEntry fileSystemEntry) {
			return new SfbSavedLocationEntry(locationType, fileSystemEntry.path, fileSystemEntry.hidden, fileSystemEntry.type);
		}

		public static SfbSavedLocationEntry FromSavedData (string data) {
			var split = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != 5) {
				Debug.LogError("Not enough arguments");
				return null;
			}

			try {
				var locationType = (SfbSavedLocationType)Enum.Parse(typeof (SfbSavedLocationType), split[0]);
				var path = split[1];
				var hidden = bool.Parse(split[2]);
				var type = (SfbFileSystemEntryType)Enum.Parse(typeof (SfbFileSystemEntryType), split[3]);
				var savedDate = DateTime.Parse(split[4]);

				return new SfbSavedLocationEntry(locationType, savedDate, path, hidden, type);
			}
			catch (Exception e) {
				Debug.LogException(e);
				return null;
			}
		}
	}
}