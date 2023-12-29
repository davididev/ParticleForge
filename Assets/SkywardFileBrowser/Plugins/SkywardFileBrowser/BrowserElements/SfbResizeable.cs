using UnityEngine;

namespace SkywardRay.FileBrowser {
	public class SfbResizeable : MonoBehaviour {
		public Vector2 minimumSize = new Vector2(200, 200);
		public Vector2 maximumSize = new Vector2(400, 400);
		public bool maxSizeCanvasSize = false;
		public bool createResizeButtons = false;
		public GameObject resizeButtonsPrefab;

		private Vector2 sizeDelta;
		private SfbResizeSide resizeSide = SfbResizeSide.None;
		private RectTransform panelRectTransform;
		private RectTransform canvasRectTransform;

		public void Start () {
			if (createResizeButtons && resizeButtonsPrefab != null) {
				GameObject go = Instantiate(resizeButtonsPrefab);
				go.name = resizeButtonsPrefab.name;
				go.transform.SetParent(transform, false);
			}
			else if (createResizeButtons) { 
				Debug.LogError("Resize buttons prefab must be set to create resize buttons.");
			}

			panelRectTransform = transform as RectTransform;
//			canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
			canvasRectTransform = transform.parent as RectTransform;
		}
		
		/// <summary>
		/// Sets the side from which to resize the window.
		/// </summary>
		/// <param name="side"> Side or corner (eg. Left, Up Right, Down Left) </param>
		public void ButtonResize (SfbResizeSide side) {
			resizeSide = side;
			sizeDelta = panelRectTransform.sizeDelta;

			if (canvasRectTransform != null && maxSizeCanvasSize) {
				maximumSize = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height);
			}
		}

		/// <summary>
		/// Stop resizing.
		/// </summary>
		public void EndResize () {
			resizeSide = SfbResizeSide.None;

			var window = GetComponent<SfbWindow>();
			if (window != null) {
				var browsers = window.GetComponentsInChildren<SfbPanel>(true);
				foreach (var browser in browsers) {
					browser.UpdateContentsAndScrollView();
				}
			}
		}

		/// <summary>
		/// Resizes window based on set resize side.
		/// </summary>
		/// <param name="pos"> New pointer position converted with ScreenPointToLocalPointInRectangle. </param>
		/// <param name="oldpos"> Old pointer position converted with ScreenPointToLocalPointInRectangle. </param>
		public void Resize (Vector2 pos, Vector2 oldpos) {
			if (panelRectTransform == null) {
				return;
			}

			Vector2 newpos = pos;
			Vector2 resizeValue = newpos - oldpos;

			var xAnchor = canvasRectTransform.rect.width * panelRectTransform.anchorMin.x + panelRectTransform.anchoredPosition.x;
			var yAnchor = panelRectTransform.anchoredPosition.y - canvasRectTransform.rect.height * (1 - panelRectTransform.anchorMin.y);

			if (resizeSide.HasSide(SfbResizeSide.Right)) {
				sizeDelta.x += resizeValue.x;
				sizeDelta.x = Mathf.Clamp(sizeDelta.x, minimumSize.x, maximumSize.x - (maxSizeCanvasSize ? xAnchor : 0));
			}
			else if (resizeSide.HasSide(SfbResizeSide.Left)) {
				sizeDelta.x -= resizeValue.x;
				sizeDelta.x = Mathf.Clamp(sizeDelta.x, minimumSize.x, xAnchor + panelRectTransform.rect.max.x);
				
				float x = sizeDelta.x - panelRectTransform.sizeDelta.x;
				panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x - x, panelRectTransform.anchoredPosition.y);
			}

			if (resizeSide.HasSide(SfbResizeSide.Bottom)) {
				sizeDelta.y += -resizeValue.y;
				sizeDelta.y = Mathf.Clamp(sizeDelta.y, minimumSize.y, maximumSize.y + (maxSizeCanvasSize ? yAnchor : 0));
			}
			else if (resizeSide.HasSide(SfbResizeSide.Top)) {
				sizeDelta.y += resizeValue.y;
				sizeDelta.y = Mathf.Clamp(sizeDelta.y, minimumSize.y, -(yAnchor + panelRectTransform.rect.min.y));

				float y = sizeDelta.y - panelRectTransform.sizeDelta.y;
				panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, panelRectTransform.anchoredPosition.y + y);
			}

			panelRectTransform.sizeDelta = sizeDelta;
		} 
	}
}