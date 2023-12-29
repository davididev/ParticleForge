using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
    public class SfbInternal : MonoBehaviour {

        public SfbSettings settings = new SfbSettings();

        public bool IsWindowOpen {
            get { return window != null && window.isActiveAndEnabled; }
        }

        public SfbMode Mode {
            get { return mode; }
        }

        public SfbFileSystemEntry CurrentDirectory {
            get { return currentDirectory; }
        }

        private SfbFileSystemEntry currentDirectory;
        private Action<string[]> outputMethod;
        private Action callbackCloseWindow;
        private SfbHistory history = new SfbHistory();
        private SfbFileSystem fileSystem = new SfbFileSystem(false);
        private List<KeyCode> heldKeys = new List<KeyCode>();
        private string[] extensions = new string[0];
        private string selectedExtension = "";
        private GameObject loadingAnimation;
        private SfbSavedLocations savedLocations = new SfbSavedLocations();
        private SfbMode mode = SfbMode.Open;
        private List<SfbPromt> openPromts = new List<SfbPromt>();
        private bool independantCanvas;
        public SfbStringEvent OnSelect = new SfbStringEvent();

        private SfbWindow window;
        private Canvas canvas;
        private GameObject tooltip;

        private bool prefabsValid;

        public void Start () {
            LoadSavedLocations();
            fileSystem.fileBrowser = this;
        }

        public bool OpenFile (string path, Action<string[]> outputMethod, string[] extensions = null) {
            mode = SfbMode.Open;
            return InitializeBrowser(path, outputMethod, null, extensions);
        }

        public bool OpenFile (string path, Action<string[]> outputMethod, Action callbackCloseWindow, string[] extensions = null) {
            mode = SfbMode.Open;
            return InitializeBrowser(path, outputMethod, callbackCloseWindow, extensions);
        }

        public bool SaveFile (string path, Action<string[]> outputMethod, string[] extensions = null) {
            mode = SfbMode.Save;
            return InitializeBrowser(path, outputMethod, null, extensions);
        }

        public bool SaveFile (string path, Action<string[]> outputMethod, Action callbackCloseWindow, string[] extensions = null) {
            mode = SfbMode.Save;
            return InitializeBrowser(path, outputMethod, callbackCloseWindow, extensions);
        }

        private bool InitializeBrowser (string path, Action<string[]> outputMethod, Action callbackCloseWindow, string[] extensions = null) {
            if (extensions != null && extensions.Length > 0) {
                var temp = extensions.Select(a => Regex.Replace(a, "^\\.{1}", "").ToLowerInvariant()).ToList();
                temp.RemoveAll(a => a == "");

                this.extensions = temp.ToArray();
            }
            else if (mode == SfbMode.Save) {
                Debug.LogError("Settings.RequireFileExtensionInSaveMode is set true, but no extensions were given as input");
                return false;
            }

            if (!OpenWindow()) {
                Debug.LogWarning("Unable to open window, closing file browser");
                return false;
            }

            if (!fileSystem.DirectoryExists(path)) {
                Debug.LogWarning("Couldn't open in directory: " + path);
                PromtWarning("Couldn't open in directory: " + path);
                path = "/";
            }

            InternalChangeDirectory(fileSystem.GetDirectory(path));

            if (settings.SaveSettingsToDisk && settings.SettingsSaveFolder == "") {
                settings.SettingsSaveFolder = Application.persistentDataPath;
                Debug.LogWarning("Problem loading from disk. SettingsSaveFolder was not set, resetting to: " + settings.SettingsSaveFolder);
            }

            LoadSavedLocations();

            this.outputMethod = outputMethod;
            this.callbackCloseWindow = callbackCloseWindow;
            locationBrowserKeepScroll = false;
            RepopulateLocationBrowserPanel();

            SetMode(mode);

            return true;
        }

        public void HideWindow () {
            if (window == null) {
                return;
            }

            window.gameObject.SetActive(false);
        }

        public void ShowWindow () {
            if (window == null) {
                return;
            }

            window.gameObject.SetActive(true);
        }

        public void FakeFileSystem (SfbFileSystem fileSystem) {
            if (!fileSystem.IsFake) {
                Debug.LogError("Fake fileSystem must be set as fake");
                return;
            }

            this.fileSystem = fileSystem;
        }

        public SfbFileSystem GetFileSystem () {
            return fileSystem;
        }

        public void SetParentCanvas (Canvas canvas) {
            if (this.canvas != null) {
                Debug.LogWarning("Input canvas was null");
            }

            if (window != null) {
                window.transform.SetParent(canvas.transform, false);
            }

            if (independantCanvas && this.canvas != null) {
                Destroy(this.canvas.gameObject);
            }

            this.canvas = canvas;
            independantCanvas = false;
        }

        public List<KeyCode> GetHeldKeys () {
            return heldKeys;
        }

        private void Update () {
            UpdateHeldKeys();
            ProcessKeyPresses();

            fileSystem.Update();
        }

        private void OnApplicationQuit () {
            SaveSavedLocations();
        }

        private void ProcessKeyPresses () {
            if (!IsWindowOpen) {
                return;
            }

            var rp = Application.platform;
            var isMac = rp == RuntimePlatform.OSXEditor || rp == RuntimePlatform.OSXPlayer;
            var ctrl = isMac
                ? heldKeys.Contains(KeyCode.LeftCommand) || heldKeys.Contains(KeyCode.RightCommand)
                : heldKeys.Any(a => a == KeyCode.LeftControl || a == KeyCode.RightControl);
            //            bool shift = heldKeys.Any(a => a == KeyCode.LeftShift || a == KeyCode.RightShift);
            var alt = heldKeys.Any(a => a == KeyCode.LeftAlt || a == KeyCode.RightAlt || a == KeyCode.AltGr);

            foreach (var key in GetDownKeys()) {
                switch (key) {
                case KeyCode.Mouse3:
                    ListenerHistoryBack();
                    break;
                case KeyCode.Mouse4:
                    ListenerHistoryForward();
                    break;
                case KeyCode.LeftArrow:
                    if (alt) {
                        ListenerHistoryBack();
                    }

                    break;
                case KeyCode.RightArrow:
                    if (alt) {
                        ListenerHistoryForward();
                    }

                    break;
                case KeyCode.A:
                    if (ctrl) {
                        SendToFocusedElement("Select All");
                    }

                    break;
                case KeyCode.Delete:
                    ListenerDeleteSelection();
                    break;
                case KeyCode.F12:
                    SendToFocusedElement("Select All");
                    break;
                }
            }
        }

        private static IEnumerable<KeyCode> GetDownKeys () {
            var keys = new List<KeyCode>();
            if (Input.anyKeyDown) {
                keys.AddRange(Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(Input.GetKeyDown));
            }

            return keys;
        }

        private void UpdateHeldKeys () {
            heldKeys.RemoveAll(Input.GetKeyUp);

            foreach (var key in GetDownKeys()) {
                if (!heldKeys.Contains(key)) {
                    heldKeys.Add(key);
                }
            }
        }

        private bool OpenWindow () {
            if (!prefabsValid && !CheckPrefabs()) {
                return false;
            }

            if (canvas == null) {
                canvas = Instantiate(prefabCanvas).GetComponent<Canvas>();
                canvas.name = "Canvas SkywardFileBrowser";
                independantCanvas = true;
            }

            if (window == null) {
                window = Instantiate(prefabWindow).GetComponent<SfbWindow>();
                window.transform.SetParent(canvas.transform, false);
                window.name = "Window";
                window.Init(this);
            }

            if (loadingAnimation == null) {
                loadingAnimation = Instantiate(prefabLoadingAnimation);
                loadingAnimation.transform.SetParent(window.transform, false);
                loadingAnimation.name = "LoadingAnimation";
                loadingAnimation.SetActive(false);
            }

            canvas.gameObject.SetActive(true);
            window.gameObject.SetActive(true);
            SetSelectedExtension();

            return true;
        }

        public void CloseWindow () {
            SaveSavedLocations();

            if (callbackCloseWindow != null) {
                callbackCloseWindow();
            }

            history.Clear();
            outputMethod = null;
            currentDirectory = null;

            if (settings.KeepBrowserInMemoryWhenClosed) {
                window.gameObject.SetActive(false);
            }
            else {
                if (independantCanvas) {
                    Destroy(canvas.gameObject);
                }
                else {
                    Destroy(window.gameObject);
                }
            }
        }

        private void InternalChangeDirectory (SfbFileSystemEntry entry) {
            currentDirectory = entry;
            fileBrowserKeepScroll = false;
            StartCoroutine(RepopulateFileBrowserPanel());

            var inputFields = window.GetComponentsInChildren<SfbInputField>(true).Where(a => a.type == SfbInputFieldType.Path);
            foreach (var field in inputFields) {
                field.SetText(currentDirectory.path);
            }

            history.Add(currentDirectory);
        }

        public void ChangeDirectory (SfbFileSystemEntry entry) {
            if (entry is SfbSavedLocationEntry) {
                entry = fileSystem.GetDirectory(entry.path);
            }

            if (entry == null) {
                return;
            }

            InternalChangeDirectory(entry);
        }

        public bool DirectoryExistsInCurrentDirectory (string input) {
            return fileSystem.DirectoryExists(currentDirectory.path + "/" + input);
        }

        public bool HasValidExtension (SfbFileSystemEntry fileSystemEntry) {
            if (fileSystemEntry == null) {
                return false;
            }

            return fileSystemEntry.type != SfbFileSystemEntryType.File || IsValidExtension(fileSystemEntry.extension);
        }

        public bool IsValidExtension (string input) {
            if (extensions.Length == 0) {
                return true;
            }

            if (input == null) {
                return false;
            }

            input = Regex.Replace(input, "^\\.{1}", "");
            input = input.ToLowerInvariant();

            if (selectedExtension == "" && (!settings.AllTypesShowOnlySetExtensions || extensions.Contains(input) || input == "")) {
                return true;
            }

            if (selectedExtension != input) {
                return false;
            }

            return extensions.Contains(input);
        }

        private bool repopulatingFileBrowser;
        private bool fileBrowserKeepScroll;

        private IEnumerator RepopulateFileBrowserPanel () {
            repopulatingFileBrowser = true;

            if (loadingAnimation != null) {
                loadingAnimation.SetActive(true);
            }

            if (currentDirectory.HasChanged()) {
                currentDirectory.readContents = false;
            }

            while (repopulatingFileBrowser) {
                if (currentDirectory.readContents) {
                    var browsers = window.GetComponentsInChildren<SfbPanel>(true).Where(a => a is SfbFilePanel).Cast<SfbFilePanel>();
                    var entries = fileSystem.GetDirectoryContents(currentDirectory, SfbFileSortingOrder.FolderThenFileThenName).Where(HasValidExtension).ToList();

                    if (!settings.ShowHiddenFiles) {
                        entries.RemoveAll(a => a.hidden);
                    }

                    foreach (var panel in browsers) {
                        panel.Repopulate(entries, fileBrowserKeepScroll);
                    }

                    repopulatingFileBrowser = false;
                }
                else {
                    fileSystem.AsyncUpdateDirectoryContents(currentDirectory);
                    yield return new WaitForSeconds(0.001f);
                }
            }

            if (loadingAnimation != null) {
                loadingAnimation.SetActive(false);
            }

            SelectionEvent();
        }

        private bool locationBrowserKeepScroll;

        private void RepopulateLocationBrowserPanel () {
            var browsers = window.GetComponentsInChildren<SfbPanel>(true).Where(a => a is SfbLocationPanel).Cast<SfbLocationPanel>();
            var entries = savedLocations.GetRecentAndFavoriteEntries().ToList();

            if (settings.RemoveLocationWhenMissing) {
                var missing = entries.Where(a => !(fileSystem.DirectoryExists(a.path) || fileSystem.FileExists(a.path))).ToList();

                foreach (var entry in missing) {
                    entries.Remove(entry);
                    savedLocations.RemoveEntry(entry);
                }
            }

            foreach (var panel in browsers) {
                panel.Repopulate(entries, locationBrowserKeepScroll);
            }

            SelectionEvent();
        }

        private void SetMode (SfbMode mode) {
            var fileNameFields = window.GetComponentsInChildren<SfbInputField>(true).Where(a => a.type == SfbInputFieldType.FileName);
            var buttons = window.GetComponentsInChildren<SfbButton>(true).Where(a => a.action == SfbButtonAction.SubmitOpenSelection || a.action == SfbButtonAction.SubmitSaveSelection);

            foreach (var field in fileNameFields) {
                field.gameObject.SetActive(mode == SfbMode.Save);
            }

            foreach (var button in buttons) {
                if (mode == SfbMode.Save) {
                    button.gameObject.SetActive(button.action == SfbButtonAction.SubmitSaveSelection);
                }
                else {
                    button.gameObject.SetActive(button.action == SfbButtonAction.SubmitOpenSelection);
                }

                if (button.action == SfbButtonAction.SubmitOpenSelection) {
                    button.GetComponent<Button>().interactable = false;
                }
            }
        }

        public void ListenerDropMenu (SfbDropMenuType type, string input) {
            switch (type) {
            case SfbDropMenuType.Extensions:
                if (input == "all types") {
                    input = "";
                }

                if (input.StartsWith(".")) {
                    input = input.Remove(0, 1);
                }

                SetSelectedExtension(input);
                break;
            }
        }

        private void SetSelectedExtension () {
            if (mode == SfbMode.Save && settings.RequireFileExtensionInSaveMode) {
                SetSelectedExtension(extensions.First());
            }
            else {
                SetSelectedExtension("");
            }
        }

        private void SetSelectedExtension (string input) {
            selectedExtension = input;
            var buttons = window.GetComponentsInChildren<SfbButton>(true).Where(a => a.action == SfbButtonAction.OpenExtensionsDropMenu);
            foreach (var button in buttons) {
                if (!button.isActiveAndEnabled || button.GetComponentInChildren<Text>() == null) {
                    return;
                }

                button.GetComponentInChildren<Text>().text = input == "" ? "all types" : "." + input;
            }

            ListenerReloadBrowsers();
        }

        public void PromtWarning (string message) {
            var bp = Instantiate(prefabPromtWarning).GetComponent<SfbPromtWarning>();
            if (bp.GetComponentInChildren<Text>() == null) {
                Debug.LogError("Warning promt window requires child with text component.");
                return;
            }

            bp.GetComponentInChildren<Text>().text = message;
            bp.Init(this);
            bp.transform.SetParent(window.transform, false);

            AddOpenPromt(bp);
        }

        public bool PromtSubmitInputField (SfbInputField inputField) {
            if (SubmitNewFolderInputField(inputField.GetText())) {
                return true;
            }

            return false;
        }

        private SfbIElement focusedElement;

        public void SetElementFocus (SfbIElement element) {
            focusedElement = element;
        }

        private void SendToFocusedElement (string message) {
            if (focusedElement == null) {
                return;
            }

            focusedElement.ReceiveMessage(message);
        }

        public void SaveSavedLocations () {
            if (settings.SaveSettingsToPlayerPrefs) {
                PlayerPrefs.SetString(settings.SettingsSaveFileName, savedLocations.FormatForSave());
                PlayerPrefs.Save();

                // Only one save setting should be used
                if (settings.SaveSettingsToDisk) {
                    Debug.LogWarning("Both save settings are enabled, saving using PlayerPrefs.");
                }
            }
            else if (settings.SaveSettingsToDisk) {
                if (settings.SettingsSaveFolder == "") {
                    Debug.LogError("File browser save path not set");
                    return;
                }

                var path = settings.SettingsSaveFolder + "/" + settings.SettingsSaveFileName;
                fileSystem.CreateBackup(path, ".bak");
                fileSystem.WriteBytesToDisk(path, Encoding.Unicode.GetBytes(savedLocations.FormatForSave()));
            }
        }

        public void LoadSavedLocations () {
            if (settings.SaveSettingsToPlayerPrefs) {
                if (!PlayerPrefs.HasKey(settings.SettingsSaveFileName)) {
                    Debug.Log("No saved settings found");
                }

                savedLocations.ParseSavedData(PlayerPrefs.GetString(settings.SettingsSaveFileName));

                // Only one save setting should be used
                if (settings.SaveSettingsToDisk) {
                    Debug.LogWarning("Both save settings are enabled, loading using PlayerPrefs.");
                }
            }
            else if (settings.SaveSettingsToDisk) {
                if (settings.SettingsSaveFolder == "") {
                    Debug.LogError("File browser save path not set");
                    return;
                }

                var path = settings.SettingsSaveFolder + "/" + settings.SettingsSaveFileName;
                var settingsEntry = fileSystem.GetFile(path);
                if (settingsEntry == null) {
                    return;
                }

                fileSystem.ReadFileOrBackupFromDisk(settingsEntry, ".bak");

                savedLocations.ParseSavedData(Encoding.Unicode.GetString(settingsEntry.FileContents));
            }
        }

        public List<SfbEntryWrapper> GetSelectedEntries () {
            var browsers = window.GetComponentsInChildren<SfbPanel>(true);
            var selectedEntries = browsers.SelectMany(a => a.GetSelected());

            return selectedEntries.ToList();
        }

        public void SelectedEntry (SfbFileSystemEntry entry) {
            if (mode == SfbMode.Save && entry.type == SfbFileSystemEntryType.File) {
                var inputFields = window.GetComponentsInChildren<SfbInputField>().Where(a => a.type == SfbInputFieldType.FileName).ToList();

                if (!inputFields.Any()) {
                    return;
                }

                inputFields.First().SetText(entry.name.Replace(entry.extension, ""));
            }
        }

        public void SelectionEvent () {
            var selectedEntries = GetSelectedEntries();
            var disabled = selectedEntries.Count == 0;

            if (settings.RestrictOutputToOneFile && selectedEntries.Count != 1) {
                disabled = true;
            }

            if (!settings.AllowFolderAsOutput && selectedEntries.Any(a => a.BrowserEntry.type == SfbEntryType.Folder || a.BrowserEntry.type == SfbEntryType.LogicalDrive)) {
                disabled = true;
            }
            else if (!settings.AllowFileAsOutput && selectedEntries.Any(a => a.BrowserEntry.type == SfbEntryType.File)) {
                disabled = true;
            }

            var buttons = window.GetComponentsInChildren<SfbButton>(true).Where(a => a.action == SfbButtonAction.SubmitOpenSelection);
            foreach (var button in buttons) {
                button.GetComponent<Button>().interactable = !disabled;
            }
        }

        public void DeselectAllEntries () {
            var browsers = window.GetComponentsInChildren<SfbPanel>(true);
            foreach (var browser in browsers) {
                browser.DeselectChildren();
            }
        }

        public void ListenerDisabledWindowPanel () {
            var panels = window.GetComponentsInChildren<SfbDisabledWindowOverlay>(true);
            foreach (var panel in panels) {
                panel.gameObject.SetActive(false);
            }

            CloseAllOpenPromts();
        }

        public void ClosingOpenPromt (SfbPromt promt) {
            openPromts.Remove(promt);

            if (openPromts.Count == 0) {
                var panels = window.GetComponentsInChildren<SfbDisabledWindowOverlay>(true);
                foreach (var panel in panels) {
                    panel.gameObject.SetActive(false);
                }
            }
        }

        public void CloseAllOpenPromts () {
            var promts = new List<SfbPromt>();
            promts.AddRange(openPromts);

            foreach (var t in promts) {
                t.Close();
            }
        }

        public void AddOpenPromt (SfbPromt promt) {
            openPromts.Add(promt);

            var panels = window.GetComponentsInChildren<SfbDisabledWindowOverlay>(true);
            foreach (var panel in panels) {
                panel.gameObject.SetActive(true);
            }
        }

        public string GetFullFileNameInput () {
            var inputFields = window.GetComponentsInChildren<SfbInputField>().Where(a => a.type == SfbInputFieldType.FileName).ToList();
            if (!inputFields.Any()) {
                return "";
            }

            var path = currentDirectory.path + "/";
            var text = inputFields.First().GetText();
            var extension = "";

            if (selectedExtension != "") {
                extension = "." + selectedExtension;
            }

            return path + text + extension;
        }

        public string GetFileNameInput () {
            var inputFields = window.GetComponentsInChildren<SfbInputField>().Where(a => a.type == SfbInputFieldType.FileName).ToList();
            if (!inputFields.Any()) {
                Debug.LogError("No inputfield found!");
                return "";
            }

            return inputFields.First().GetText();
        }

        public void ShowTooltip (SfbFileSystemEntry entry, PointerEventData eventData) {
            if (tooltip == null) {
                tooltip = Instantiate(prefabTooltip);
                tooltip.name = "Tooltip";
                tooltip.transform.SetParent(window.transform, false);
            }

            tooltip.SetActive(true);
            tooltip.GetComponentInChildren<Text>().text = entry.name;

            if (settings.LockTooltipToCursor) {
                var rt = (RectTransform)tooltip.transform;
                Vector2 point;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(window.transform as RectTransform, Input.mousePosition, eventData.enterEventCamera, out point);
                rt.anchoredPosition = point + new Vector2(10, 0);
            }
        }

        public void HideTooltip () {
            if (tooltip != null) {
                tooltip.SetActive(false);
            }
        }

        #region Prefabs

        public GameObject prefabCanvas;
        public GameObject prefabWindow;
        public GameObject prefabPromtDelete;
        public GameObject prefabPromtNewFolder;
        public GameObject prefabPromtOverwrite;
        public GameObject prefabPromtWarning;
        public GameObject prefabLoadingAnimation;
        public GameObject prefabTooltip;

        private bool CheckPrefabs () {
            prefabsValid = true;

            // Canvas
            if (prefabCanvas == null || prefabCanvas.GetComponent<Canvas>() == null) {
                prefabsValid = false;
                Debug.LogError("Canvas prefab was not set correctly");
            }

            // Window
            if (prefabWindow == null || prefabWindow.GetComponent<SfbWindow>() == null) {
                prefabsValid = false;
                Debug.LogError("Window prefab was not set correctly");
            }

            // Promts
            if (prefabPromtDelete == null || prefabPromtDelete.GetComponent<SfbPromtDelete>() == null) {
                prefabsValid = false;
                Debug.LogError("Delete promt prefab was not set correctly");
            }

            if (prefabPromtNewFolder == null || prefabPromtNewFolder.GetComponent<SfbPromtNewFolder>() == null) {
                prefabsValid = false;
                Debug.LogError("New folder promt prefab was not set correctly");
            }

            if (prefabPromtOverwrite == null || prefabPromtOverwrite.GetComponent<SfbPromtOverwrite>() == null) {
                prefabsValid = false;
                Debug.LogError("Overwrite promt prefab was not set correctly");
            }

            if (prefabPromtWarning == null || prefabPromtWarning.GetComponent<SfbPromtWarning>() == null) {
                prefabsValid = false;
                Debug.LogError("Warning promt prefab was not set correctly");
            }

            // Loading Animation
            if (prefabLoadingAnimation == null) {
                prefabsValid = false;
                Debug.LogError("Loading animation prefab was not set correctly");
            }

            return prefabsValid;
        }

        #endregion

        #region ButtonListeners

        public void SetButtonListeners (Button button, SfbButtonAction action) {
            button.onClick.AddListener(() => ListenerClick(action));
        }

        private void ListenerClick (SfbButtonAction action) {
            switch (action) {
            case SfbButtonAction.HistoryBack:
                ListenerHistoryBack();
                break;
            case SfbButtonAction.HistoryForward:
                ListenerHistoryForward();
                break;
            case SfbButtonAction.OpenParentDirectory:
                ListenerOpenParentDirectory();
                break;
            case SfbButtonAction.ReloadBrowsers:
                ListenerReloadBrowsers();
                break;
            case SfbButtonAction.ShowDrives:
                InternalChangeDirectory(fileSystem.root);
                break;
            case SfbButtonAction.AddToFavorites:
                ListenerAddToFavorites();
                break;
            case SfbButtonAction.NewFolder:
                ListenerNewFolder();
                break;
            case SfbButtonAction.DeleteSelection:
                ListenerDeleteSelection();
                break;
            case SfbButtonAction.OpenExtensionsDropMenu:
                ListenerOpenExtensionsDropMenu();
                break;
            case SfbButtonAction.CloseBrowser:
                CloseWindow();
                break;
            case SfbButtonAction.SubmitOpenSelection:
                ListenerSubmitOpenSelection();
                break;
            case SfbButtonAction.SubmitSaveSelection:
                ListenerSubmitSaveFile();
                break;
            case SfbButtonAction.OpenDesktopDirectory:
                ListenerDesktopButton();
                break;
            case SfbButtonAction.OpenHomeDirectory:
                ListenerHomeButton();
                break;
            }
        }

        private void ListenerHistoryBack () {
            while (true) {
                var entry = history.Previous();
                if (entry == null) {
                    break;
                }

                if (!fileSystem.DirectoryExists(entry)) {
                    history.ReportInvalidEntry(entry);
                }
                else {
                    history.Back();
                    InternalChangeDirectory(history.Current());
                    break;
                }
            }
        }

        private void ListenerHistoryForward () {
            while (true) {
                var entry = history.Next();
                if (entry == null) {
                    break;
                }

                if (!fileSystem.DirectoryExists(entry)) {
                    history.ReportInvalidEntry(entry);
                }
                else {
                    history.Forward();
                    InternalChangeDirectory(history.Current());
                    break;
                }
            }
        }

        private void ListenerOpenParentDirectory () {
            var directory = fileSystem.GetParentDirectory(currentDirectory.path);

            if (directory != null) {
                InternalChangeDirectory(directory);
            }
            else {
                Debug.LogWarning("No parent directory");
            }
        }

        private void ListenerDesktopButton () {
            var path = SfbFileSystem.GetNormalizedPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            ChangeDirectory(fileSystem.GetDirectory(path));
        }

        private void ListenerHomeButton () {
            var path = SfbFileSystem.GetNormalizedPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            ChangeDirectory(fileSystem.GetDirectory(path));
        }

        private void ListenerReloadBrowsers () {
            if (currentDirectory == null) {
                return;
            }

            fileBrowserKeepScroll = true;
            locationBrowserKeepScroll = true;

            StartCoroutine(RepopulateFileBrowserPanel());
            RepopulateLocationBrowserPanel();
        }

        private void ListenerAddToFavorites () {
            var selectedEntries = GetSelectedEntries();

            if (!selectedEntries.Any()) {
                return;
            }

            foreach (var entry in selectedEntries) {
                savedLocations.AddFavoriteEntry(entry.BrowserEntry.fileSystemEntry);
            }

            locationBrowserKeepScroll = true;
            RepopulateLocationBrowserPanel();
        }

        private void ListenerNewFolder () {
            var bp = Instantiate(prefabPromtNewFolder).GetComponent<SfbPromtNewFolder>();
            if (bp.GetComponentInChildren<SfbInputField>() == null) {
                Debug.LogError("New Folder browser promt window requires inputfield with: BrowserPromtInputField component");
                return;
            }

            bp.GetComponentInChildren<SfbInputField>().type = SfbInputFieldType.NewFolder;
            bp.transform.SetParent(window.transform, false);
            bp.Init(this);

            AddOpenPromt(bp);
        }

        public void ListenerNewFolderConfirm (string input) {
            SubmitNewFolderInputField(input);
        }

        public void ListenerSubmitOpenSelection () {
            var selectedEntries = GetSelectedEntries();

            if (!selectedEntries.Any()) {
                return;
            }

            if (settings.RestrictOutputToOneFile && selectedEntries.Count > 1) {
                PromtWarning("Please select only one item.");
                return;
            }

            if (!settings.AllowFolderAsOutput && selectedEntries.Any(a => a.BrowserEntry.type == SfbEntryType.Folder || a.BrowserEntry.type == SfbEntryType.LogicalDrive)) {
                PromtWarning("Please select files only.");
                return;
            }

            if (!settings.AllowFileAsOutput && selectedEntries.Any(a => a.BrowserEntry.type == SfbEntryType.File)) {
                PromtWarning("Please select folders only.");
                return;
            }

            outputMethod.Invoke(selectedEntries.Select(a => a.BrowserEntry.fileSystemEntry.path).ToArray());

            if (!selectedEntries.All(a => a.fileSystemEntry is SfbSavedLocationEntry)) {
                savedLocations.AddRecentEntry(currentDirectory);
            }

            CloseWindow();
        }

        public void ListenerSubmitSaveFile () {
            var filename = GetFullFileNameInput();

            if (GetFileNameInput() == "") {
                PromtWarning("Please enter a file name");
                return;
            }

            if (fileSystem.FileExists(filename)) {
                var bp = Instantiate(prefabPromtOverwrite).GetComponent<SfbPromtOverwrite>();
                bp.transform.SetParent(window.transform, false);
                bp.Init(this);

                AddOpenPromt(bp);
                return;
            }

            ListenerSubmitSaveFileConfirm();
        }

        public void ListenerSubmitSaveFileConfirm () {
            outputMethod.Invoke(new[] {GetFullFileNameInput()});
            savedLocations.AddRecentEntry(currentDirectory);
            CloseWindow();
        }

        private void ListenerOpenExtensionsDropMenu () {
            if (extensions.Length <= 1) {
                return;
            }

            var dropMenus = window.GetComponentsInChildren<SfbDropMenu>(true).Where(a => a.type == SfbDropMenuType.Extensions).ToList();
            foreach (var dropMenu in dropMenus) {
                dropMenu.gameObject.SetActive(true);
                var items = extensions.Select(a => "." + a).ToList();
                if (mode == SfbMode.Open || !settings.RequireFileExtensionInSaveMode) {
                    items.Add("all types");
                }

                dropMenu.Repopulate(items);
            }
        }

        private void ListenerDeleteSelection () {
            if (!window.GetComponentsInChildren<SfbPanel>(true).SelectMany(a => a.GetSelected().Select(b => b.fileSystemEntry)).Any()) {
                return;
            }

            var bp = Instantiate(prefabPromtDelete).GetComponent<SfbPromtDelete>();
            bp.transform.SetParent(window.transform, false);
            bp.Init(this);

            AddOpenPromt(bp);
        }

        public void ListenerDeleteSelectionConfirm () {
            var selected = window.GetComponentsInChildren<SfbPanel>(true).SelectMany(a => a.GetSelected().Select(b => b.fileSystemEntry));
            foreach (var entry in selected) {
                var locationEntry = entry as SfbSavedLocationEntry;
                
                if (locationEntry != null) {
                    savedLocations.RemoveEntry(locationEntry);
                }
                else {
                    fileSystem.DeleteEntryOnDiskAndRemove(entry);

                    if (settings.RemoveLocationOnDelete) {
                        savedLocations.RemoveEntry(entry.path);
                    }
                }
            }

            ListenerReloadBrowsers();
        }

        #endregion

        #region InputFieldListeners

        public void SubmitPathInputField (string input) {
            input = input.Replace("\\", "/");

            if (fileSystem.DirectoryExists(input)) {
                ChangeDirectory(fileSystem.GetDirectory(input));
            }
        }

        public bool SubmitNewFolderInputField (string input) {
            if (!fileSystem.DirectoryExists(currentDirectory.path + "/" + input)) {
                fileSystem.NewDirectory(currentDirectory.path + "/", input);

                fileBrowserKeepScroll = true;
                StartCoroutine(RepopulateFileBrowserPanel());
                return true;
            }

            PromtWarning("Directory already exists.");
            return false;
        }

        public void SubmitFileNameInputField (string input) {
            ListenerSubmitSaveFile();
        }

        #endregion

    }
}
