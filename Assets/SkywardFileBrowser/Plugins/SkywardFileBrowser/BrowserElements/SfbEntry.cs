using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SkywardRay.FileBrowser {
	public class SfbEntry : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
		public SfbEntryType type;
		public Graphic targetGraphic;
		public SfbFileSystemEntry fileSystemEntry;
		public Color normalColor = new Color32(255, 255, 255, 0);
		public Color highlightedColor = new Color32(255, 255, 255, 255);
		public Color selectedColor = new Color32(240, 240, 240, 255);
		public Color pressedColor = new Color32(200, 200, 200, 255);
		public float fadeDuration = 0.1f;
		public bool Selected { get { return _selected; } set { _selected = value; } }
		public bool Pressed { get { return _pressed; } set { _pressed = value; } }
		private bool _selected = false;
		private bool _pressed = false;

		private bool isPointerDown;
		private bool isPointerInside;
		private State state = State.Normal;

		internal SfbEntryWrapper wrapper;
		private SfbPanel parentPanel;

		private void Start () {
			if (targetGraphic == null && GetComponent<Image>() != null) {
				targetGraphic = GetComponent<Image>();
			}

			parentPanel = wrapper.GetComponentInParent<SfbPanel>();
			StartColorTween(normalColor, true);
		}

		private void Update () {
			UpdateState();
		}

		private void OnEnable () {
			UpdateColor(true);
		}

		private void UpdateState () {
			State newState = State.Normal;

			if (!Pressed) {
				if (isPointerInside && type != SfbEntryType.HeaderFavorite && type != SfbEntryType.HeaderRecent) {
					newState = isPointerDown ? State.Pressed : State.Highlighted;
				}

				if (newState == State.Normal && Selected) {
					newState = State.Selected;
				}
			}
			else {
				newState = State.Pressed;
			}

			if (state != newState) {
				state = newState;
				UpdateColor();
			}
		}

		private void UpdateColor (bool instant = false) {
			switch (state) {
				case State.Normal:
					StartColorTween(normalColor, instant);
					break;
				case State.Highlighted:
					StartColorTween(highlightedColor, instant);
					break;
				case State.Selected:
					StartColorTween(selectedColor, instant);
					break;
				case State.Pressed:
					StartColorTween(pressedColor, instant);
					break;
			}
		}

		private void StartColorTween (Color targetColor, bool instant) {
			if (targetGraphic == null) {
				return;
			}

			targetGraphic.CrossFadeColor(targetColor, instant ? 0f : fadeDuration, true, true);
		}

		public void OnPointerDown (PointerEventData eventData) {
			isPointerDown = true;
			parentPanel.PointerDownOnChild(this);
		}

		public void OnPointerUp (PointerEventData eventData) {
			isPointerDown = false;
			parentPanel.PointerUpOnChild(this);
		}

		public void OnPointerEnter (PointerEventData eventData) {
			isPointerInside = true;
			parentPanel.PointerEnterOnChild(this, eventData);
		}

		public void OnPointerExit (PointerEventData eventData) {
			isPointerInside = false;
			parentPanel.PointerExitOnChild(this);
		}

		public void OnDrop (PointerEventData eventData) {
			parentPanel.DropOnChild(this);
		}

		private enum State {
			Normal,
			Highlighted,
			Selected,
			Pressed,
		}
	}
}