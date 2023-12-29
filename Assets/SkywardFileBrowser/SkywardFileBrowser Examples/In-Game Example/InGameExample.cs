using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using SkywardRay.FileBrowser;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SkywardRay.FileBrowser.Example {
	public class InGameExample : MonoBehaviour {
		public GameObject prefabBrowser;
		public Button switchButton;
		public InputField extensionInputField;
		public Canvas customCanvas;
		public bool setCustomCanvas = true;
		public TextAsset FakeFileSystemJSON;
		public bool setFakeFileSystem = true;

		private SkywardFileBrowser fileBrowser;
		private string defaultPath = "/";
		private string[] extensions = { "jpg", ".png", "TXT" };
		private Stopwatch restartTimer;

		private void Start () {
			// Add a listener to the switch button so we can switch the File Browser mode with it
			if (switchButton != null) {
				switchButton.onClick.AddListener(SwitchButton);
			}

			// Add a listener to the inputfield so we can update the extensions
			if (extensionInputField != null) {
				extensionInputField.onEndEdit.AddListener(OnSubmitExtensions);
			}

			// Create the File Browser
			fileBrowser = Instantiate(prefabBrowser).GetComponent<SkywardFileBrowser>();

			// Change some settings
			fileBrowser.Settings.RequireFileExtensionInSaveMode = true;
			fileBrowser.Settings.ShowHiddenFiles = true;

			// Set a canvas for the File Browser
			if (setCustomCanvas && customCanvas != null) {
				fileBrowser.SetParentCanvas(customCanvas);
			}

			// Set a fake file system so we can simulate the file system of our in-game computer
			if (setFakeFileSystem && FakeFileSystemJSON != null) {
				fileBrowser.FakeFileSystem(SfbFileSystem.CreateFromJSON(FakeFileSystemJSON.text));
			}

			// There are multiple examples in the asset, so in order to give each file browser it's own settings file
			fileBrowser.Settings.SettingsSaveFileName = "SfbInGameExampleSettings";

			// Open the File Browser
			OpenFileBrowser(SfbMode.Open, defaultPath, Output, extensions);
		}

		private void Update () {
			// Quit the demo by pressing escape
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}

			// Restart the browser when it is closed, so we don't have to reopen the demo
			ReOpenFileBrowser();
		}

		// Opens the File Browser in the specified mode
		private void OpenFileBrowser (SfbMode mode, string path, Action<string[]> outputCallback, string[] extensions = null) {
			if (mode == SfbMode.Save) {
				fileBrowser.SaveFile(path, outputCallback, extensions);
			}
			else {
				fileBrowser.OpenFile(path, outputCallback, extensions);
			}
		}

		private void ReOpenFileBrowser () {
			// Don't start the timer when the file browser is still open
			if (!fileBrowser.IsWindowOpen && restartTimer == null) {
				restartTimer = Stopwatch.StartNew();
			}
			if (!fileBrowser.IsWindowOpen && restartTimer.ElapsedMilliseconds > 500) {
				// Reopen the File Browser
				var mode = fileBrowser.Mode;
				var path = fileBrowser.GetCurrentDirectoryPath();
				OpenFileBrowser(mode, path, Output, extensions);

				restartTimer = null;
			}
		}

		// The function recieving the output from the filebrowser
		private void Output (string[] output) {
			// Simply print the paths to show something happened. 
			// Replace this with your own code to use the File Browser output
			foreach (string path in output) {
				Debug.Log("File browser output: " + path);
			}
		}

		// The listener for the switch button
		private void SwitchButton () {
			var mode = fileBrowser.Mode == SfbMode.Open ? SfbMode.Save : SfbMode.Open;

			// Open in the same folder
			var path = fileBrowser.GetCurrentDirectoryPath();

			OpenFileBrowser(mode, path, Output, extensions);
		}

		// Parse the extensions and update the File Browser
		private void OnSubmitExtensions (string input) {
			// Seperate by spaces
			var split = input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			// Any string with or without a dot and 
			extensions = split.Where(s => Regex.IsMatch(s, "[\\.a-zA-Z]*")).ToArray();

			// Reopen the File Browser
			var mode = fileBrowser.Mode;
			var path = fileBrowser.GetCurrentDirectoryPath();
			OpenFileBrowser(mode, path, Output, extensions);
		}
	}
}