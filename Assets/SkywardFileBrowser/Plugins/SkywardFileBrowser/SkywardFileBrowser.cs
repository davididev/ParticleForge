using System;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SkywardFileBrowser : MonoBehaviour {
		public GameObject prefabCanvas;
		public GameObject prefabWindow;
		public GameObject prefabPromtDelete;
		public GameObject prefabPromtNewFolder;
		public GameObject prefabPromtOverwrite;
		public GameObject prefabPromtWarning;
		public GameObject prefabLoadingAnimation;
		public GameObject prefabTooltip;

		public SfbStringEvent OnSelect {
			get { return SfbInternal.OnSelect; }
		}

		private SfbInternal _mSfbInternal;

		private SfbInternal SfbInternal {
			get {
				if (_mSfbInternal == null) {
					_mSfbInternal = gameObject.AddComponent<SfbInternal>();

					_mSfbInternal.prefabCanvas = prefabCanvas;
					_mSfbInternal.prefabWindow = prefabWindow;
					_mSfbInternal.prefabPromtDelete = prefabPromtDelete;
					_mSfbInternal.prefabPromtNewFolder = prefabPromtNewFolder;
					_mSfbInternal.prefabPromtOverwrite = prefabPromtOverwrite;
					_mSfbInternal.prefabPromtWarning = prefabPromtWarning;
					_mSfbInternal.prefabLoadingAnimation = prefabLoadingAnimation;
					_mSfbInternal.prefabTooltip = prefabTooltip;
				}

				return _mSfbInternal;
			}
		}

		public bool IsWindowOpen {
			get { return SfbInternal.IsWindowOpen; }
		}

		public SfbMode Mode {
			get { return SfbInternal.Mode; }
		}

		public SfbSettings Settings {
			get { return SfbInternal.settings; }
		}

		public bool OpenFile (string path, Action<string[]> outputMethod, string[] extensions = null) {
			return SfbInternal.OpenFile(path, outputMethod, extensions);
		}

		public bool OpenFile (string path, Action<string[]> outputMethod, Action callbackCloseWindow, string[] extensions = null) {
			return SfbInternal.OpenFile(path, outputMethod, callbackCloseWindow, extensions);
		}

		public bool SaveFile (string path, Action<string[]> outputMethod, string[] extensions = null) {
			return SfbInternal.SaveFile(path, outputMethod, extensions);
		}

		public bool SaveFile (string path, Action<string[]> outputMethod, Action callbackCloseWindow, string[] extensions = null) {
			return SfbInternal.SaveFile(path, outputMethod, callbackCloseWindow, extensions);
		}

		public void HideWindow () {
			SfbInternal.HideWindow();
		}

		public void ShowWindow () {
			SfbInternal.ShowWindow();
		}

		public void FakeFileSystem (SfbFileSystem fileSystem) {
			SfbInternal.FakeFileSystem(fileSystem);
		}

		public void SetParentCanvas (Canvas canvas) {
			SfbInternal.SetParentCanvas(canvas);
		}

		public string GetCurrentDirectoryPath () {
			return SfbInternal.CurrentDirectory != null ? SfbInternal.CurrentDirectory.path : "/";
		}
	}
}