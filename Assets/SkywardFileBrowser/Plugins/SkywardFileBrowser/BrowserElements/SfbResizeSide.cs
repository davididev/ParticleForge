namespace SkywardRay.FileBrowser {
	public enum SfbResizeSide {
		None,
		Bottom,
		Left,
		Right,
		Top,
		BottomLeft,
		BottomRight,
		TopLeft,
		TopRight,
	}

	static class SfbResizeSideExtensions {
		/// <summary>
		/// Used to see if a side is included in a combination. eg (BottomRight.HasSide(Right) => true, BottomRight.HasSide(Top) => false)
		/// </summary>
		/// <param name="side"> The side the function is called on. </param>
		/// <param name="other"> A side (Left, Right, Bottom, Top). Combinations, eg (Top Left) will return false. </param>
		public static bool HasSide (this SfbResizeSide side, SfbResizeSide other) {
			switch (side) {
				case SfbResizeSide.Left:
					return other == SfbResizeSide.Left;
				case SfbResizeSide.TopLeft:
					return other == SfbResizeSide.Left || other == SfbResizeSide.Top;
				case SfbResizeSide.BottomLeft:
					return other == SfbResizeSide.Left || other == SfbResizeSide.Bottom;
				case SfbResizeSide.Right:
					return other == SfbResizeSide.Right;
				case SfbResizeSide.TopRight:
					return other == SfbResizeSide.Right || other == SfbResizeSide.Top;
				case SfbResizeSide.BottomRight:
					return other == SfbResizeSide.Right || other == SfbResizeSide.Bottom;
				case SfbResizeSide.Bottom:
					return other == SfbResizeSide.Bottom;
				case SfbResizeSide.Top:
					return other == SfbResizeSide.Top;
				default:
					return false;
			}
		}
	}
}