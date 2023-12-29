


--- General use ---

As shown in the example scenes. Have a script that creates an instance of "Plugins/SkywardFileBrowser/Prefabs/SkywardFileBrowser.prefab" or use another method to have a reference to a SkywardFileBrowser component. In your script, call SkywardFileBrowser.OpenFile() or SkywardFileBrowser.SaveFile(). These functions require a path of the initial folder it will open and a callback function that will recieve the output. Like this:

void OutputExample (string[] s) {
	// Your code here.
}

The file browser will call this function when the user presses the confirm button on the browser (open/save). The output is an array of paths to the files and/or folders that were selected or the path of the file that sould be saved.



--- Saved Locations (Recent and favorite files and folders) ---

The file browser saves the Saved Locations to the Unity PlayerPrefs by default, this is a setting you can turn off. The key used is 'SfbSettings'

Alternatively you can set SaveSettingsToPlayerPrefs to false and SaveSettingsToDisk to true to get the file browser to save its settings to disk. This does require you to set the directory for the save file using SetSettingsDirectoryPath() or the setting FileBrowserSavePath. This will create a file called 'SkywardFileBrowser.settings'.

If you leave both of these options enabled, the file browser will give a warning and use the PlayerPrefs. If you don't use either one the file browser will not be able to save and load the Saved Locations when the the application is restarted.



--- Settings ---

The file browser uses a class called SfbSettings to allow some customization in its inner workings. A setting can be changed by calling [file browser reference].Settings.[setting] = [new value]. As can be seen in the example scene scripts. Here is a list of all settings and a short description for each:

// Set to false if you don't want the user to be able to select folders as output.
AllowFolderAsOutput; default = true

// Set to false if you don't want the user to be able to select files as output.
AllowFolderAsOutput; default = true

// The path where the file browser will save its SkywardFileBrowser.settings file when SaveSettingToDisk is true.
FileBrowserSavePath; default = ""

// The opacity of hidden entries.
HiddenOpacity; default = 0.55

// The maximum allowed recent entries to save, the oldest one will be removed when there are too many.
MaxRecentEntries; default = 5

// The time between clicks in milliseconds that will register the second click as the user double clicking.
DoubleClickTime; default = 300

// Set to false to allow users to submit a filename without an extension.
RequireFileExtensionInSaveMode; default = false

// Set to true to delete a saved location when the file it references can't be found.
RemoveLocationWhenMissing; default = false

// Set to true to delete a saved location when the file or folder is deleted from within the file browser.
RemoveLocationOnDelete; default = true

// Set to true to make the file browser save its settings to the key 'SfbSettings' in playerprefs
SaveSettingsToPlayerPrefs; default = true

// Set to true to make the file browser save its settings to disk in the folder set in FileBrowserSavePath
SaveSettingsToDisk; default = false

// Set to true to show hidden file.
ShowHiddenFiles; default = false

// The time a user has to hover the mouse over an entry before a tooltip is shown. The tooltip only shows on files and folders that have names too long for the window
TooltipDelay; default = 500