using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbSavedLocations {
		private Dictionary<string, SfbSavedLocationEntry> recentList = new Dictionary<string, SfbSavedLocationEntry>();
		private Dictionary<string, SfbSavedLocationEntry> favoriteList = new Dictionary<string, SfbSavedLocationEntry>();
		public uint maxRecentEntries = 5;

		public void AddRecentEntry (SfbFileSystemEntry fileSystemEntry) {
			RemoveOldestRecent();

			if (!recentList.ContainsKey(fileSystemEntry.path)) {
				recentList.Add(fileSystemEntry.path, SfbSavedLocationEntry.FromFileSystemEntry(SfbSavedLocationType.Recent, fileSystemEntry));
			}
		}

		public void AddRecentEntry (SfbSavedLocationEntry savedLocationEntry) {
			RemoveOldestRecent();

			if (savedLocationEntry.locationType != SfbSavedLocationType.Recent) {
				return;
			}
			if (!recentList.ContainsKey(savedLocationEntry.path)) {
				recentList.Add(savedLocationEntry.path, savedLocationEntry);
			}
		}

		public void RemoveRecentEntry (string path) {
			if (recentList.ContainsKey(path)) {
				recentList.Remove(path);
			}
		}

		public void RemoveOldestRecent () {
			var sorted = recentList.Values.OrderByDescending(a => a.savedDate);

			while (recentList.Count >= maxRecentEntries) {
				recentList.Remove(sorted.Last().path);
			}
		}

		public void AddFavoriteEntry (SfbFileSystemEntry fileSystemEntry) {
			if (!favoriteList.ContainsKey(fileSystemEntry.path)) {
				favoriteList.Add(fileSystemEntry.path, SfbSavedLocationEntry.FromFileSystemEntry(SfbSavedLocationType.Favorite, fileSystemEntry));
			}
		}

		public void AddFavoriteEntry (SfbSavedLocationEntry savedLocationEntry) {
			if (savedLocationEntry.locationType != SfbSavedLocationType.Favorite) {
				return;
			}
			if (!favoriteList.ContainsKey(savedLocationEntry.path)) {
				favoriteList.Add(savedLocationEntry.path, savedLocationEntry);
			}
		}

		public void RemoveFavoriteEntry (string path) {
			if (favoriteList.ContainsKey(path)) {
				favoriteList.Remove(path);
			}
		}

		public void AddEntry (SfbSavedLocationEntry savedLocationEntry) {
			if (savedLocationEntry.locationType == SfbSavedLocationType.Recent) {
				AddRecentEntry(savedLocationEntry);
			}
			else {
				AddFavoriteEntry(savedLocationEntry);
			}
		}

		public void RemoveEntry (SfbSavedLocationEntry savedLocationEntry) {
			if (savedLocationEntry.locationType == SfbSavedLocationType.Recent) {
				RemoveRecentEntry(savedLocationEntry.path);
			}
			else {
				RemoveFavoriteEntry(savedLocationEntry.path);
			}
		}

		public void RemoveEntry (string path) {
			RemoveFavoriteEntry(path);
			RemoveRecentEntry(path);
		}

		public IEnumerable<SfbSavedLocationEntry> GetRecentEntries () {
			return recentList.Values;
		}

		public IEnumerable<SfbSavedLocationEntry> GetRecentAndFavoriteEntries () {
			var combined = recentList.Values.ToList();
			combined.AddRange(favoriteList.Values);
			return combined;
		}

		public string FormatForSave () {
			string val = "SavedLocations {\n";

			val = recentList.Values.Aggregate(val, (current, entry) => current + entry.FormatForSave());
			val = favoriteList.Values.Aggregate(val, (current, entry) => current + entry.FormatForSave());

			return val + "}\n";
		}

		public void ParseSavedData (string data) {
			data = data.Replace("\n", "");

			foreach (Match match in Regex.Matches(data, "(?<=SavedLocationEntry {)(.*?)(?=})")) {
				SfbSavedLocationEntry entry = SfbSavedLocationEntry.FromSavedData(match.Groups[1].Value);
				if (entry != null) {
					AddEntry(entry);
				}
			}
		}
	}
}