using UnityEngine;
using SkywardRay.FileBrowser;
using System.Collections;

public class OpenWithButton : MonoBehaviour {
	public GameObject prefabBrowser;

	private SkywardFileBrowser fileBrowser;
	private string defaultPath = "/";
	private string[] extensions = { "jpg", ".png", "TXT" };

	private void Start () {
		// Create the File Browser
		fileBrowser = Instantiate(prefabBrowser).GetComponent<SkywardFileBrowser>();

		// You can change setting here or leave this part out
		fileBrowser.Settings.RequireFileExtensionInSaveMode = true;
		fileBrowser.Settings.ShowHiddenFiles = true;

		// To give this file browser it's own settings file
		fileBrowser.Settings.SettingsSaveFileName = "MyFilebrowserSettings";
	}
	
	// Opens the File Browser in the specified mode
	public void OpenFileBrowser () {
		fileBrowser.OpenFile(defaultPath, Output, ClosedWindow, extensions);
	}

	// The function recieving the output from the filebrowser
	private void Output (string[] output) {
		// Simply print the paths to show something happened. 
		// Replace this with your own code to use the File Browser output
		foreach (string path in output) {
			Debug.Log("File browser output: " + path);
		}
	}

	// The function called when the file browser is closed
	private void ClosedWindow () {
		// Print a message to show the closing event
		Debug.Log("File browser window closed");
	}
}
