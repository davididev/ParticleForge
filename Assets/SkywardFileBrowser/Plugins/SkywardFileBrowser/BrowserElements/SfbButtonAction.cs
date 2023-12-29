using System;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public enum SfbButtonAction {
		NOT_SET,

		HistoryBack,
		HistoryForward,
		OpenParentDirectory,
		ReloadBrowsers,
		ShowDrives,

		NewFolder,
		AddToFavorites,
		DeleteSelection,

		OpenExtensionsDropMenu,

		SubmitOpenSelection,
		SubmitSaveSelection,
		CloseBrowser,

		OpenDesktopDirectory,
		OpenHomeDirectory,
	}
}