
#if !UNITY_WEBGL && !UNITY_WEBPLAYER

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbFileSystemThread {
		public bool IsAlive { get; private set; }
		public bool IsWorking { get; private set; }

		private Thread thread;
		private Action queuedAction = null;
		private SfbFileSystemEntry[] output;
		private Action<string, SfbFileSystemEntry[]> callbackAction;
		private bool waitForCallback = false;
		private string path = "";

		private readonly object tLock = new object();
		private readonly object sharedMonitor = new object();

		public SfbFileSystemThread () {
			IsAlive = true;

			thread = new Thread(ThreadFunction);
			thread.IsBackground = true;
			thread.Start();
		}

		public void MainThreadUpdate () {
			bool invoke = false;
			SfbFileSystemEntry[] entries;

			lock (tLock) {
				invoke = waitForCallback;
				entries = output;
			}

			if (invoke) {
				callbackAction.Invoke(path, entries);

				lock (tLock) {
					waitForCallback = false;
				}
			}
		}

		public void AsyncReadDirectoryContents (string path, Action<string, SfbFileSystemEntry[]> callback) {
			lock (tLock) {
				if (IsWorking) {
					return;
				}

				this.path = path;
				queuedAction = ReadDirectoryContents;
				callbackAction = callback;
			}

			lock (sharedMonitor) {
				Monitor.Pulse(sharedMonitor);
			}
		}

		#region Threading

		public void KillThreadAndWait () {
			IsAlive = false;

			lock (sharedMonitor) {
				Monitor.Pulse(sharedMonitor);
			}

			while (thread.IsAlive) {}
		}

		private void InvokeCallbackOnMainThread () {
			lock (tLock) {
				waitForCallback = true;
			}
		}

		private void ThreadFunction () {
			Action currentAction = null;

			while (IsAlive) {
				bool done = false;

				lock (tLock) {
					if (waitForCallback) {
						done = true;
					}
					else if (currentAction == null) {
						if (queuedAction == null) {
							done = true;
						}
						else {
							currentAction = queuedAction;
							queuedAction = null;
							IsWorking = true;
						}
					}
				}

				if (done) {
					lock (tLock) {
						IsWorking = false;
					}
					lock (sharedMonitor) {
						Monitor.Wait(sharedMonitor);
					}
				}
				else {
					try {
						currentAction.Invoke();
						currentAction = null;
					}
					catch (Exception e) {
						Debug.LogException(e);
					}
					finally {
						InvokeCallbackOnMainThread();
					}
				}
			}
		}

		#endregion

		#region AsyncFunctions

		private void ReadDirectoryContents () {
			string directory;

			lock (tLock) {
				directory = path;
			}

			List<SfbFileSystemEntry> entries = new List<SfbFileSystemEntry>();

			if (!Directory.Exists(directory)) {
				return;
			}

			foreach (string f in Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)) {
				var info = new FileInfo(f);
				bool hidden = (info.Attributes & FileAttributes.Hidden) != 0;
				entries.Add(new SfbFileSystemEntry(f, hidden, SfbFileSystemEntryType.File));
			}
			foreach (string d in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)) {
				var info = new DirectoryInfo(d);

				if ((info.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) {
					continue;
				}

				bool hidden = (info.Attributes & FileAttributes.Hidden) != 0;
				entries.Add(new SfbFileSystemEntry(d, hidden, SfbFileSystemEntryType.Folder));
			}

			lock (tLock) {
				output = entries.ToArray();
			}
		}

		#endregion
	}
}

#endif