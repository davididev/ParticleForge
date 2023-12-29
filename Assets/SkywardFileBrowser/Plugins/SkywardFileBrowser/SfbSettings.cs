using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbSettings {
		private string _settingsSaveFolder;
		private float _hiddenOpacity;

		public SfbSettings () {
			AllTypesShowOnlySetExtensions = true;
			AllowFolderAsOutput = true;
			AllowFileAsOutput = true;
			AlwaysShowScrollbar = false;
			_settingsSaveFolder = "";
			_hiddenOpacity = 0.55f;
			MaxRecentEntries = 5;
			DoubleClickTime = 300;
			KeepBrowserInMemoryWhenClosed = true;
			LockTooltipToCursor = true;
			RequireFileExtensionInSaveMode = false;
			RemoveLocationWhenMissing = false;
			RemoveLocationOnDelete = true;
			RestrictOutputToOneFile = false;
			SaveSettingsToPlayerPrefs = true;
			SaveSettingsToDisk = false;
			SettingsSaveFileName = "SfbSettings";
			ShowHiddenFiles = false;
			TooltipDelay = 500;
		}

		public bool AllTypesShowOnlySetExtensions { get; set; }
		public bool AllowFolderAsOutput { get; set; }
		public bool AllowFileAsOutput { get; set; }
		public bool AlwaysShowScrollbar { get; set; }
		public uint DoubleClickTime { get; set; }
		public bool KeepBrowserInMemoryWhenClosed { get; set; }
		public bool LockTooltipToCursor { get; set; }
		public uint MaxRecentEntries { get; set; }
		public bool RemoveLocationOnDelete { get; set; }
		public bool RemoveLocationWhenMissing { get; set; }
		public bool RestrictOutputToOneFile { get; set; }
		public bool RequireFileExtensionInSaveMode { get; set; }
		public bool SaveSettingsToPlayerPrefs { get; set; }
		public bool SaveSettingsToDisk { get; set; }
		public string SettingsSaveFileName { get; set; }
		public bool ShowHiddenFiles { get; set; }
		public uint TooltipDelay { get; set; }

		public string SettingsSaveFolder {
			get { return _settingsSaveFolder; }
			set {
				_settingsSaveFolder = SfbFileSystem.GetNormalizedPath(value);
				if (_settingsSaveFolder == "/") {
					_settingsSaveFolder = "";
				}
			}
		}

		public float HiddenOpacity {
			get { return _hiddenOpacity; }
			set { _hiddenOpacity = Mathf.Clamp01(value); }
		}
	}
}