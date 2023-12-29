using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbFileSystemEntry {
		public bool readContents = false;
		public readonly bool hidden;
		public readonly string path;
		public readonly string name;
		public readonly string extension;
		public readonly SfbFileSystemEntryType type;
		public SfbFileSystemEntry parent;
		public List<SfbFileSystemEntry> children = new List<SfbFileSystemEntry>();
		public byte[] FileContents { get; private set; }
		public DateTime lastWriteTime;

		public SfbFileSystemEntry (string path, bool hidden, SfbFileSystemEntryType type) {
			path = SfbFileSystem.GetNormalizedPath(path);
			this.path = path;
			this.type = type;
			this.hidden = hidden;

			name = SfbFileSystem.GetFileName(path);
			if (type == SfbFileSystemEntryType.File) {
				extension = SfbFileSystem.GetExtension(path);
			}

			ReadLastWriteTime();
		}

		public void AddChild (SfbFileSystemEntry entry) {
			if (!children.Contains(entry)) {
				entry.parent = this;
				children.Add(entry);
			}
		}

		public void RemoveChild (SfbFileSystemEntry entry) {
			if (children.Contains(entry)) {
				children.Remove(entry);
			}
		}

		public void ReadLastWriteTime () {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			lastWriteTime = type == SfbFileSystemEntryType.File ? new FileInfo(path).LastWriteTime : new DirectoryInfo(path).LastWriteTime;
#endif
		}

		public bool HasChanged () {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			var newTime = type == SfbFileSystemEntryType.File ? new FileInfo(path).LastWriteTime : new DirectoryInfo(path).LastWriteTime;
			return newTime != lastWriteTime;
#else
			return false;
#endif
		}

		public void SetContents (byte[] bytes) {
			FileContents = bytes;
		}

		public void ReadContentsFromDisk () {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (type != SfbFileSystemEntryType.File || !File.Exists(path)) {
				return;
			}

			FileContents = File.ReadAllBytes(path);

			readContents = true;
#endif
		}

		public void WriteContentsToDisk () {
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
			if (type != SfbFileSystemEntryType.File) {
				return;
			}
			if (FileContents == null) {
				Debug.LogWarning("No contents to write to disk");
				return;
			}

			File.WriteAllBytes(path, FileContents);
#endif
		}
	}
}