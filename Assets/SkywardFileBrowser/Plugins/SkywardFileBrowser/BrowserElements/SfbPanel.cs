using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SkywardRay.FileBrowser {
    public abstract class SfbPanel : MonoBehaviour, SfbIElement, IPointerDownHandler, IPointerUpHandler, IDropHandler {

        public Transform content;
        public Color EntryAlternateBackground = new Color(1, 1, 1, 0);

        protected readonly List<SfbEntry> entryPrefabs = new List<SfbEntry>();
        protected SfbInternal fileBrowser;
        private SfbDraggable parentDraggable;
        protected Scrollbar scrollbar;
        protected ScrollRect scrollRect;
        private Stopwatch hoverTimer = new Stopwatch();
        private SfbEntry hoverChild;
        private PointerEventData hoverEvent;
        protected float lastScrollPosition = 1;

        protected readonly List<SfbEntryWrapper> wrappers = new List<SfbEntryWrapper>();

        private void Update () {
            if (hoverTimer.IsRunning) {
                if (hoverTimer.ElapsedMilliseconds >= fileBrowser.settings.TooltipDelay && hoverChild != null) {
                    var text = hoverChild.GetComponentInChildren<Text>();

                    if (text != null && text.preferredWidth > text.rectTransform.rect.width) {
                        fileBrowser.ShowTooltip(hoverChild.fileSystemEntry, hoverEvent);
                    }

                    hoverTimer.Stop();
                }
            }
        }

        public IEnumerator UpdateScrollView () {
            if (scrollbar == null) {
                scrollbar = GetComponentInChildren<Scrollbar>();
            }

            yield return null;

            var contentRectTransform = content.transform as RectTransform;
            if (contentRectTransform == null || scrollRect == null || !(scrollRect.transform is RectTransform)) {
                yield break;
            }

            scrollRect.verticalNormalizedPosition = lastScrollPosition;

            if (fileBrowser.settings.AlwaysShowScrollbar || contentRectTransform.rect.height > ((RectTransform)scrollRect.transform).rect.height) {
                scrollbar.gameObject.SetActive(true);
                scrollRect.verticalNormalizedPosition = lastScrollPosition;
            }
            else {
                scrollbar.gameObject.SetActive(false);
            }
        }

        protected void Repopulate (IEnumerable<SfbFileSystemEntry> entries, bool keepScrollPosition) {
            lastScrollPosition = scrollRect.verticalNormalizedPosition;

            if (entryPrefabs.Count == 0) {
                return;
            }

            if (!keepScrollPosition) {
                lastScrollPosition = 1;
            }

            for (var i = 0; i < content.childCount; i++) {
                Destroy(content.GetChild(i).gameObject);
            }

            CreateWrappers(entries);
            StartShowOnScreenEntries(lastScrollPosition);

            StartCoroutine(UpdateScrollView());
        }

        public void UpdateContentsAndScrollView () {
            lastScrollPosition = scrollRect.verticalNormalizedPosition;
            StartShowOnScreenEntries(lastScrollPosition);
            StartCoroutine(UpdateScrollView());
        }

        private void CreateWrappers (IEnumerable<SfbFileSystemEntry> entries) {
            wrappers.Clear();

            foreach (var entry in entries) {
                var entry1 = entry;
                var wrapper = SfbEntryWrapper.CreateEmpty(((RectTransform)entryPrefabs.First(a => a.type == entry1.type.Convert()).transform).rect.height);
                wrapper.fileSystemEntry = entry;
                wrapper.rectTransform.SetParent(content, false);
                wrapper.parent = this;
                wrappers.Add(wrapper);
            }
        }

        protected void StartShowOnScreenEntries (float scrollPosition) {
            StartCoroutine(ShowOnScreenEntries(scrollPosition));
        }

        private IEnumerator ShowOnScreenEntries (float scrollPosition) {
            if (scrollRect == null) {
                scrollRect = GetComponentInChildren<ScrollRect>();

                if (scrollRect == null) {
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame();

            scrollPosition = -scrollPosition + 1;

            var scrollRectHeight = ((RectTransform)scrollRect.transform).rect.height;
            var contentHeight = scrollRect.content.rect.height;

            var lowest = -1;
            var highest = -1;
            float currentHeightOffset = 0;
            var minVisible = (contentHeight - scrollRectHeight) * scrollPosition;
            var maxVisible = Mathf.Clamp(minVisible + scrollRectHeight, scrollRectHeight, contentHeight);
            for (var i = 0; i < wrappers.Count; i++) {
                currentHeightOffset += ((RectTransform)wrappers[i].transform).rect.height;

                if (currentHeightOffset > minVisible && lowest == -1) {
                    lowest = i;
                }

                if (currentHeightOffset > maxVisible && highest == -1) {
                    highest = i;
                    break;
                }
            }

            if (lowest == -1) {
                lowest = 0;
            }

            if (highest == -1) {
                highest = wrappers.Count - 1;
            }

            for (var i = 0; i < wrappers.Count; i++) {
                if (i >= lowest && i <= highest) {
                    if (wrappers[i].BrowserEntry == null) {
                        wrappers[i].BrowserEntry = BrowserEntryFromFileSystemEntry(wrappers[i], i % 2 == 1);
                    }
                    else {
                        wrappers[i].EntryActive = true;
                    }
                }
                else {
                    wrappers[i].EntryActive = false;
                }
            }
        }

        private SfbEntry BrowserEntryFromFileSystemEntry (SfbEntryWrapper wrapper, bool alternate) {
            var go = Instantiate(entryPrefabs.First(a => a.type == wrapper.fileSystemEntry.type.Convert()).gameObject);
            go.name = wrapper.fileSystemEntry.name.Replace("/", "\\");
            go.transform.SetParent(wrapper.transform, false);

            go.GetComponentInChildren<Text>().text = wrapper.fileSystemEntry.name;

            if (wrapper.fileSystemEntry.hidden) {
                var images = go.GetComponentsInChildren<Image>();
                if (images.Length >= 2) {
                    images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b,
                        fileBrowser.settings.HiddenOpacity);
                }

                var text = go.GetComponentInChildren<Text>();
                if (text != null) {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, fileBrowser.settings.HiddenOpacity);
                }
            }

            var entry = go.GetComponent<SfbEntry>();
            entry.fileSystemEntry = wrapper.fileSystemEntry;
            entry.wrapper = wrapper;

            if (alternate) {
                entry.normalColor = EntryAlternateBackground;
            }

            return entry;
        }

        public List<SfbEntryWrapper> GetSelected () {
            return wrappers.Where(a => a.Selected).ToList();
        }

        private Stopwatch clickStopwatch;
        private SfbEntryWrapper lastClickedWrapper;

        private void Click (SfbEntryWrapper wrapper) {
            var rp = Application.platform;
            var isMac = rp == RuntimePlatform.OSXEditor || rp == RuntimePlatform.OSXPlayer;
            var ctrl = isMac
                ? fileBrowser.GetHeldKeys().Contains(KeyCode.LeftCommand) || fileBrowser.GetHeldKeys().Contains(KeyCode.RightCommand)
                : fileBrowser.GetHeldKeys().Any(a => a == KeyCode.LeftControl || a == KeyCode.RightControl);

            if (clickStopwatch.ElapsedMilliseconds < fileBrowser.settings.DoubleClickTime) {
                DoubleClick(wrapper);
                clickStopwatch = Stopwatch.StartNew();
                return;
            }

            clickStopwatch = Stopwatch.StartNew();
            if (ctrl) {
                wrapper.Selected = !wrapper.Selected;
                if (wrapper.Selected) {
                    lastClickedWrapper = wrapper;
                }
            }
            else if (fileBrowser.GetHeldKeys().Any(a => a == KeyCode.LeftShift || a == KeyCode.RightShift)) {
                if (lastClickedWrapper != null && lastClickedWrapper.Selected) {
                    GetChildRange(wrapper, lastClickedWrapper).ForEach(a => a.Selected = true);
                    fileBrowser.SelectionEvent();
                }
            }
            else {
                lastClickedWrapper = wrapper;
                var newState = true;

                if (wrappers.Count(a => a.BrowserEntry != null && a.Selected) <= 1) {
                    newState = !wrapper.Selected;
                }

                fileBrowser.DeselectAllEntries();
                wrapper.Selected = newState;

                fileBrowser.SelectedEntry(wrapper.fileSystemEntry);
                fileBrowser.SelectionEvent();

                if (wrapper.fileSystemEntry.type == SfbFileSystemEntryType.File) {
                    fileBrowser.OnSelect.Invoke(wrapper.fileSystemEntry.path);
                }
            }
        }

        private void DoubleClick (SfbEntryWrapper wrapper) {
            if (wrapper.BrowserEntry.type == SfbEntryType.Folder) {
                fileBrowser.ChangeDirectory(wrapper.fileSystemEntry);
            }
            else if (wrapper.BrowserEntry.type == SfbEntryType.File) {
                if (fileBrowser.Mode == SfbMode.Open) {
                    fileBrowser.ListenerSubmitOpenSelection();
                }
                else {
                    fileBrowser.ListenerSubmitSaveFile();
                }
            }

            wrapper.Selected = false;
            fileBrowser.SelectionEvent();
        }

        public void DeselectChildren () {
            wrappers.ForEach(a => a.Selected = false);
            fileBrowser.SelectionEvent();
        }

        private SfbEntry selectedChild;

        public void PointerDownOnChild (SfbEntry child) {
            selectedChild = child;
        }

        public void PointerUpOnChild (SfbEntry child) {
            SetFocus();

            // Check for mouse button up because OnPointerUp seems to get called even when the mouse button is down
            if (child == selectedChild && Input.GetMouseButtonUp(0)) {
                Click(child.wrapper);
            }

            selectedChild = null;
        }

        public void DropOnChild (SfbEntry child) { }

        public void PointerEnterOnChild (SfbEntry child, PointerEventData eventData) {
            if (Input.touchCount != 0) {
                return;
            }

            hoverTimer = Stopwatch.StartNew();
            hoverChild = child;
            hoverEvent = eventData;
        }

        public void PointerExitOnChild (SfbEntry child) {
            if (Input.touchCount != 0) {
                return;
            }

            if (hoverChild != null) {
                hoverTimer.Stop();
                hoverChild = null;
                fileBrowser.HideTooltip();
            }
        }

        private List<SfbEntryWrapper> GetChildRange (SfbEntryWrapper first, SfbEntryWrapper last) {
            var lo = Mathf.Min(wrappers.IndexOf(first), wrappers.IndexOf(last));
            var hi = Mathf.Max(wrappers.IndexOf(first), wrappers.IndexOf(last));
            return wrappers.GetRange(lo, hi + 1 - lo);
        }

        public void OnPointerDown (PointerEventData eventData) {
            if (parentDraggable != null) {
                // This is here to allow dragging on browser panels to move the window
                parentDraggable.OnPointerDown(eventData);
            }
        }

        public void OnPointerUp (PointerEventData eventData) {
            wrappers.ForEach(a => a.Selected = false);
            SetFocus();

            selectedChild = null;
            fileBrowser.SelectionEvent();
        }

        public virtual void Init (SfbInternal fileBrowser) {
            this.fileBrowser = fileBrowser;
            clickStopwatch = Stopwatch.StartNew();

            gameObject.SetActive(true);

            scrollRect = GetComponentInChildren<ScrollRect>();
            parentDraggable = GetComponentInParent<SfbDraggable>();

            if (scrollRect.verticalScrollbar != null) {
                scrollRect.verticalScrollbar.onValueChanged.AddListener(StartShowOnScreenEntries);
            }
        }

        public void SetFocus () {
            fileBrowser.SetElementFocus(this);
        }

        public void ReceiveMessage (string message) {
            if (message == "Select All") {
                wrappers.ForEach(a => a.Selected = true);
                fileBrowser.SelectionEvent();
            }
        }

        public void OnDrop (PointerEventData eventData) {
            if (selectedChild == null) {
                return;
            }

            Debug.Log("OnDrop on panel");
            var selection = !selectedChild.Selected;
            foreach (var wrapper in wrappers.Where(a => a.Pressed)) {
                wrapper.Pressed = false;
                wrapper.Selected = selection;
            }

            selectedChild = null;
            fileBrowser.SelectionEvent();
        }

    }
}
