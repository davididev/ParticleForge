#pragma warning disable 162

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleJSON;
using Debug = UnityEngine.Debug;

namespace SkywardRay.FileBrowser {
	public class SfbFileSystem {
		public bool IsFake { get; private set; }
		private Dictionary<string, SfbFileSystemEntry> entries = new Dictionary<string, SfbFileSystemEntry>();
		public SfbFileSystemEntry root;
		public SfbInternal fileBrowser;

#if !UNITY_WEBGL && !UNITY_WEBPLAYER
		private SfbFileSystemThread thread;
#endif

		public SfbFileSystem (bool isFake) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			thread = new SfbFileSystemThread();
#endif

			IsFake = isFake;

			entries.Add("/", new SfbFileSystemEntry("/", false, SfbFileSystemEntryType.Root));
			root = entries["/"];

			if (isFake == false) {
				ReadLogicalDrives();
			}
		}

		public void Update () {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			thread.MainThreadUpdate();
#endif
		}

		public void AddEntry (SfbFileSystemEntry entry) {
			if (entry.parent == null) {
				if (entry.type == SfbFileSystemEntryType.LogicalDrive) {
					entry.parent = root;
					root.AddChild(entry);
				}
				else {
					string s = GetParentPath(entry.path);
					if (entries.ContainsKey(s)) {
						entry.parent = entries[s];
						entries[s].AddChild(entry);
					}
				}
			}

			var children = entries.Where(a => GetParentPath(a.Key) == entry.path).Select(b => b.Value);
			foreach (var child in children) {
				entry.AddChild(child);
			}

			if (!entries.ContainsKey(entry.path)) {
				entries.Add(entry.path, entry);
			}
		}

		public void RemoveEntry (SfbFileSystemEntry entry) {
			entry.children.RemoveAll(a => a == entry);
			if (entry.parent != null) {
				entry.parent.RemoveChild(entry);
			}
			else {
				string s = GetParentPath(entry.path);
				Debug.Log(entries.ContainsKey(s));
			}
			entries.Remove(entry.path);
		}

		public void DeleteEntryOnDiskAndRemove (SfbFileSystemEntry entry) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (IsFake) {
				return;
			}
			if (!File.Exists(entry.path) && !Directory.Exists(entry.path)) {
				return;
			}

			FileAttributes attr = File.GetAttributes(entry.path);

			if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
				Directory.Delete(entry.path, true);
			}
			else {
				File.Delete(entry.path);
			}

			RemoveEntry(entry);
#endif
		}

		public void NewDirectory (string path) {
			if (DirectoryExists(path)) {
				return;
			}
			path = GetNormalizedPath(path);
			if (!IsFake) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
				Directory.CreateDirectory(path);
#endif
			}
			AddEntry(new SfbFileSystemEntry(path, false, SfbFileSystemEntryType.Folder));
		}

		public void NewDirectory (string parentPath, string name) {
			parentPath = GetNormalizedPath(parentPath);

			NewDirectory(parentPath + "/" + name);
		}

		public static bool RealDirectoryExists (string path) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			return Directory.Exists(path) && (new DirectoryInfo(path).Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint;
#else
			return false;
#endif
		}

		public bool DirectoryExists (string path) {
			if (IsFake) {
				return entries.ContainsKey(path);
			}

			return RealDirectoryExists(path);
		}

		public bool DirectoryExists (SfbFileSystemEntry entry) {
			return entries.ContainsKey(entry.path) && entries[entry.path] == entry;
		}

		public bool FileExists (string path) {
			if (IsFake) {
				return !entries.ContainsKey(path);
			}
			if (entries.ContainsKey(path)) {
				return true;
			}

#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			return File.Exists(path);
#else
			return false;
#endif
		}

		public bool FileExists (SfbFileSystemEntry entry) {
			if (entry.type == SfbFileSystemEntryType.File) {
				return false;
			}

			return entries.ContainsKey(entry.path) && entries[entry.path] == entry;
		}

		public SfbFileSystemEntry GetDirectory (string path) {
			if (!entries.ContainsKey(path)) {
				if (IsFake) {
					return null;
				}

				return ReadDirectory(path);
			}

			return entries[path];
		}

		public SfbFileSystemEntry GetFile (string path) {
			if (!entries.ContainsKey(path)) {
				return IsFake ? null : ReadFile(path);
			}

			return entries[path];
		}

		public List<SfbFileSystemEntry> GetDirectoryContents (SfbFileSystemEntry entry) {
			if (!entries.ContainsKey(entry.path)) {
				return new List<SfbFileSystemEntry>();
			}

			if (entry.type == SfbFileSystemEntryType.Root) {
				if (root.children.Count == 0) {
					ReadLogicalDrives();
				}
				return root.children;
			}

			return entry.children;
		}

		public List<SfbFileSystemEntry> GetDirectoryContents (SfbFileSystemEntry entry, SfbFileSortingOrder sortingOrder) {
			switch (sortingOrder) {
				case SfbFileSortingOrder.FolderThenFileThenName:
					return GetDirectoryContents(entry).OrderByDescending(a => a.type).ThenBy(b => b.name).ToList();
			}

			return null;
		}

		public static char[] GetInvalidFileNameChars () {
			//#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			return Path.GetInvalidFileNameChars();
			//#else
			//			return new char[0];
			//#endif
		}

		public static string GetExtension (string path) {
			//#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			return Path.GetExtension(path);
			//#else
			//			return "";
			//#endif
		}

		public static string GetFileName (string path) {
			string s = path;
			if (s.EndsWith("/")) {
				s = path.Remove(s.Length - 1);
			}
			s = Regex.Match(s, @"([^/]*$)").Groups[1].Value;

			if (s == "" || Regex.IsMatch(s, @"[a-zA-Z]:")) {
				s += "/";
			}

			return s;
		}

		public static string GetParentPath (string path) {
			if (path.EndsWith("/")) {
				path = path.Remove(path.Length - 1);
			}
			path = Regex.Replace(path, @"[^/]*$", "");
			if (path == "") {
				path = "/";
			}
			return GetNormalizedPath(path);
		}

		public static string GetNormalizedPath (string path) {
			path = path.Replace('\\', '/');

			if (path.EndsWith("/")) {
				path = path.Remove(path.Length - 1);
			}

			if (path == "" || Regex.IsMatch(path, @"[a-zA-Z]:$")) {
				path += "/";
			}

			return path;
		}

		public static SfbFileSystem CreateFromJSON (string json) {
			var N = JSON.Parse(json);
			var fileSystem = new SfbFileSystem(true);

			if (N["fileSystem"] != null && N["fileSystem"]["root"] != null) {
				ParseJSONInto(N["fileSystem"]["root"], "/", fileSystem);
			}
			else {
				Debug.Log("No root found: " + N.ToString(""));
			}

			return fileSystem;
		}

		public static void ParseJSONInto (JSONNode N, string parentPath, SfbFileSystem fileSystem) {
			if (N["contents"] == null || N["contents"].Count == 0) {
				return;
			}

			foreach (var child in N["contents"].Childs) {
				if (child["logicalDrive"] != null) {
					if (child["logicalDrive"]["name"] == null) {
						return;
					}

					var path = parentPath + child["logicalDrive"]["name"].Value + "/";

					fileSystem.AddEntry(new SfbFileSystemEntry(path, false, SfbFileSystemEntryType.LogicalDrive));
					ParseJSONInto(child["logicalDrive"], path, fileSystem);
				}
				if (child["folder"] != null) {
					if (child["folder"]["name"] == null) {
						return;
					}

					var path = parentPath + child["folder"]["name"].Value + "/";
					var hidden = child["folder"]["hidden"] != null && child["folder"]["hidden"].AsBool;

					fileSystem.AddEntry(new SfbFileSystemEntry(path, hidden, SfbFileSystemEntryType.Folder));
					ParseJSONInto(child["folder"], path, fileSystem);
				}
				if (child["file"] != null) {
					if (child["file"]["name"] == null || child["file"]["extension"] == null) {
						return;
					}

					var path = parentPath + child["file"]["name"].Value + "." + child["file"]["extension"].Value;
					var hidden = child["file"]["hidden"] != null && child["file"]["hidden"].AsBool;

					fileSystem.AddEntry(new SfbFileSystemEntry(path, hidden, SfbFileSystemEntryType.File));
				}
			}
		}

		public SfbFileSystemEntry GetParentDirectory (string path) {
			if (entries.ContainsKey(path) && entries[path].parent != null) {
				return entries[path].parent;
			}

			if (path.EndsWith("/")) {
				path = path.Remove(path.Length - 1);
			}
			path = Regex.Replace(path, @"[^/]*$", "");
			if (path == "") {
				path = "/";
			}

			if (entries.ContainsKey(path)) {
				return entries[path];
			}
			if (IsFake) {
				return null;
			}

			return ReadDirectory(path);
		}

		private SfbFileSystemEntry ReadFile (string path) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (IsFake) {
				return null;
			}
			string normalizedPath = GetNormalizedPath(path);
			if (!File.Exists(normalizedPath)) {
				return null;
			}
			SfbFileSystemEntry entry;

			if (!entries.ContainsKey(normalizedPath)) {
				bool hidden = (File.GetAttributes(normalizedPath) & FileAttributes.Hidden) != 0;
				entry = new SfbFileSystemEntry(normalizedPath, hidden, SfbFileSystemEntryType.File);
				AddEntry(entry);
			}
			else {
				entry = entries[normalizedPath];
			}
			return entry;
#else
			return null;
#endif
		}

		private SfbFileSystemEntry ReadDirectory (string path) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (IsFake) {
				return null;
			}
			string normalizedPath = GetNormalizedPath(path);
			SfbFileSystemEntry entry;

			if (!entries.ContainsKey(normalizedPath)) {
				bool hidden = (new DirectoryInfo(normalizedPath).Attributes & FileAttributes.Hidden) != 0;
				entry = new SfbFileSystemEntry(normalizedPath, hidden, SfbFileSystemEntryType.Folder);
				AddEntry(entry);
			}
			else {
				entry = entries[normalizedPath];
			}
			return entry;
#else
			return null;
#endif
		}

		private void ReadLogicalDrives () {
#if UNITY_ANDROID
			AsyncUpdateDirectoryContents(root);
			return;
#endif
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (IsFake) {
				return;
			}

			var drives = new List<string>();
			foreach (string drive in Environment.GetLogicalDrives()) {
				bool accessible = true;
				try {
					Directory.GetFileSystemEntries(drive);
				}
				catch (Exception) {
					accessible = false;
				}
				if (accessible) {
					string normalizedPath = drive.Replace('\\', '/');
					drives.Add(normalizedPath);
				}
			}

			var remove = root.children.Select(a => a.path).Except(drives);
			foreach (var s in remove) {
				RemoveEntry(entries[s]);
			}

			foreach (var drive in drives) {
				if (entries.ContainsKey(drive)) {
					continue;
				}

				var entry = new SfbFileSystemEntry(drive, false, SfbFileSystemEntryType.LogicalDrive);
				AddEntry(entry);
			}

			root.readContents = true;
#endif
		}

		public SfbFileSystemEntry CreateNewFileAndAddEntry (string path) {
			if (entries.ContainsKey(path)) {
				Debug.LogWarning("File already exists");
				return null;
			}

			SfbFileSystemEntry entry = new SfbFileSystemEntry(path, false, SfbFileSystemEntryType.File);

#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (!IsFake) {
				try {
					File.Create(path);
				}
				catch (Exception e) {
					Debug.LogException(e);
					Debug.LogError("Could not create file");
					return null;
				}
			}
#endif

			AddEntry(entry);
			return entry;
		}

		public void ReadFileOrBackupFromDisk (SfbFileSystemEntry fileSystemEntry, string backupExtension) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (string.IsNullOrEmpty(backupExtension)) {
				Debug.LogError("Cannot read backup file bacause no backup extension was set.");
				return;
			}
			if (fileSystemEntry.type != SfbFileSystemEntryType.File) {
				return;
			}

			if (!backupExtension.StartsWith(".")) {
				backupExtension = "." + backupExtension;
			}

			if (!File.Exists(fileSystemEntry.path)) {
				if (File.Exists(fileSystemEntry.path + backupExtension)) {
					fileSystemEntry.SetContents(File.ReadAllBytes(fileSystemEntry.path + backupExtension));
				}

				return;
			}

			fileSystemEntry.ReadContentsFromDisk();
#endif
		}

		public void CreateBackup (string path, string backupExtension) {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (string.IsNullOrEmpty(backupExtension)) {
				Debug.LogError("Cannot write backup file bacause no backup extension was set.");
				return;
			}
			if (!backupExtension.StartsWith(".")) {
				backupExtension = "." + backupExtension;
			}
			if (!File.Exists(path)) {
				return;
			}
			string backupPath = path + backupExtension;

			try {
				if (File.Exists(backupPath)) {
					File.Move(backupPath, backupPath + ".temp");
				}

				File.Move(path, backupPath);

				if (File.Exists(backupPath + ".temp")) {
					File.Delete(backupPath + ".temp");
				}

				var backupEntry = GetFile(backupPath) ?? new SfbFileSystemEntry(backupPath, false, SfbFileSystemEntryType.File);
				AddEntry(backupEntry);
			}
			catch (Exception e) {
				Debug.LogException(e);
			}
#endif
		}

		public void WriteBytesToDisk (string path, byte[] bytes) {
			path = GetNormalizedPath(path);

			var entry = GetFile(path) ?? new SfbFileSystemEntry(path, false, SfbFileSystemEntryType.File);

			entry.SetContents(bytes);
			entry.WriteContentsToDisk();
		}

		private bool IsUpdatingDirectoryContents;

		public void AsyncUpdateDirectoryContents (SfbFileSystemEntry fileSystemEntry) {
			if (IsFake) {
				fileSystemEntry.readContents = true;
				return;
			}

#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (fileSystemEntry.readContents || IsUpdatingDirectoryContents) {
				return;
			}
			if (fileSystemEntry.type == SfbFileSystemEntryType.Root) {
#if UNITY_ANDROID
				thread.AsyncReadDirectoryContents("/", AsyncRecieveDirectoryContents);
				IsUpdatingDirectoryContents = true;
				return;
#endif
				ReadLogicalDrives();
				return;
			}


			thread.AsyncReadDirectoryContents(fileSystemEntry.path, AsyncRecieveDirectoryContents);
			IsUpdatingDirectoryContents = true;
#endif
		}

		private void AsyncRecieveDirectoryContents (string path, SfbFileSystemEntry[] contents) {
			path = GetNormalizedPath(path);
			if (!entries.ContainsKey(path)) {
				fileBrowser.PromtWarning("Entry does not exist");
				Debug.Log("entry does not exist");
				return;
			}
			var directory = entries[path];

			if (contents == null) {
				directory.ReadLastWriteTime();
				directory.readContents = true;
				IsUpdatingDirectoryContents = false;
				fileBrowser.PromtWarning("No contents");
				return;
			}

			foreach (var entry in contents) {
				if (entries.ContainsKey(entry.path)) {
					directory.AddChild(entries[entry.path]);
					continue;
				}

				directory.AddChild(entry);
				entries.Add(entry.path, entry);
			}

			directory.ReadLastWriteTime();
			directory.readContents = true;
			IsUpdatingDirectoryContents = false;
		}
	}
}