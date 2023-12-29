namespace SkywardRay.FileBrowser {
	public enum SfbFileSystemEntryType {
		Root,
		File,
		Folder,
		LogicalDrive,
	}

	static class SfbFileSystemEntryTypeExtensions {
		public static SfbEntryType Convert (this SfbFileSystemEntryType fde) {
			switch (fde) {
				case SfbFileSystemEntryType.File:
					return SfbEntryType.File;
				case SfbFileSystemEntryType.Folder:
				case SfbFileSystemEntryType.LogicalDrive:
				case SfbFileSystemEntryType.Root:
					return SfbEntryType.Folder;
			}

			return SfbEntryType.File;
		}
	}
}