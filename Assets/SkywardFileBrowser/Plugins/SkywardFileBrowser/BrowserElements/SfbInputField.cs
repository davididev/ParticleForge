using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace SkywardRay.FileBrowser {
	public class SfbInputField : MonoBehaviour {
		public SfbInputFieldType type;
		public string text = "New Folder";

		private SfbInternal fileBrowser;
		private InputField inputField;
		private string defaultText;

		private static char[] invalidCharsPath;
		// Only used in error messages
		private static char[] invalidCharsFileName;

		private void Start () {
			Init();
		}

		public void Init () {
			defaultText = text;
			SetText(defaultText);
		}

		public bool IsTextDefault () {
			return defaultText == text;
		}

		public void SetText (string s) {
			if (invalidCharsPath == null) {
				invalidCharsPath = SfbFileSystem.GetInvalidFileNameChars().ToList().Where(a => a != '/' && a != '\\').ToArray();
			}
			if (invalidCharsFileName == null) {
				// Remove control characters. This way the user is only notified about characters they can remove.
				invalidCharsFileName = SfbFileSystem.GetInvalidFileNameChars().ToList().Where(a => !char.IsControl(a)).ToArray();
			}
			if (inputField == null) {
				inputField = GetComponent<InputField>();
				inputField.onEndEdit.AddListener(OnSubmit);
			}
			if (fileBrowser == null) {
				fileBrowser = GetComponentInParent<SfbWindow>().fileBrowser;
			}

			text = s;
			inputField.MoveTextStart(false);
			inputField.text = s;
			inputField.MoveTextEnd(true);
		}

		public string GetText () {
			return inputField.text;
		}

		public void Submit () {
			InternalOnSubmit(GetText());
		}

		public void OnSubmit (string input) {
			if (!Input.GetButtonDown("Submit")) {
				return;
			}

			InternalOnSubmit(input);
		}

		private void InternalOnSubmit (string input) {
			if (IsValidInput(input)) {
				switch (type) {
					case SfbInputFieldType.Path:
						fileBrowser.SubmitPathInputField(input);
						break;
					case SfbInputFieldType.NewFolder:
						fileBrowser.ListenerNewFolderConfirm(input);
						break;
					case SfbInputFieldType.FileName:
						fileBrowser.SubmitFileNameInputField(input);
						break;
				}
			}
			else {
				if (type == SfbInputFieldType.Path) {
					fileBrowser.PromtWarning("Path cannot include the following characters: " + new string(invalidCharsFileName).Replace("/", ""));
					return;
				}

				fileBrowser.PromtWarning("Input cannot include the following characters: " + new string(invalidCharsFileName));
				return;
			}
		}

		public bool IsValidInput (string input) {
			if (type == SfbInputFieldType.Path) {
				return Regex.Replace(input, "[a-zA-Z]:[/\\\\]", "").IndexOfAny(invalidCharsPath) == -1;
			}

			return input.IndexOfAny(SfbFileSystem.GetInvalidFileNameChars()) == -1;
		}
	}
}