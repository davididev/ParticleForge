using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbHistory {
		private List<SfbFileSystemEntry> history = new List<SfbFileSystemEntry>();
		private int currentIndex = -1;

		public SfbFileSystemEntry Current () {
			if (history.Count == 0) {
				return null;
			}
			if (currentIndex == -1) {
				return history.Last();
			}

			return history[currentIndex];
		}

		public void Add (SfbFileSystemEntry entry) {
			if (entry == Current()) {
				return;
			}

			if (currentIndex != -1) {
				history.RemoveRange(currentIndex + 1, history.Count - (currentIndex + 1));
				currentIndex = -1;
			}

			history.Add(entry);
		}

		public SfbFileSystemEntry Previous () {
			if (history.Count < 2) {
				return null;
			}
			if (currentIndex == -1) {
				return history[history.Count - 2];
			}
			if (currentIndex - 1 >= 0) {
				return history[currentIndex - 1];
			}
			return null;
		}

		public SfbFileSystemEntry Next () {
			if (history.Count < 2 || currentIndex == -1) {
				return null;
			}
			if (currentIndex + 1 < history.Count) {
				return history[currentIndex + 1];
			}
			return null;
		}

		public void ReportInvalidEntry (SfbFileSystemEntry entry) {
			for (int i = 0; i < history.Count; i++) {
				if (history[i] == entry) {
					history.RemoveAt(i);

					if (history.Count == 0) {
						currentIndex = -1;
						break;
					}
					if (i == currentIndex && i >= history.Count) {
						currentIndex = -1;
						break;
					}
					if (i < currentIndex) {
						currentIndex--;
					}
				}
			}
		}

		public void Back () {
			if (history.Count < 2) {
				return;
			}

			if (currentIndex == -1) {
				currentIndex = history.Count - 2;
			}
			else {
				if (currentIndex - 1 < 0) {
					return;
				}
				currentIndex--;
			}
		}

		public void Forward () {
			if (currentIndex == -1) {
				return;
			}

			if (currentIndex + 1 >= history.Count) {
				currentIndex = -1;
			}
			else {
				currentIndex++;
			}
		}

		public void Clear () {
			currentIndex = -1;
			history.Clear();
		}
	}
}