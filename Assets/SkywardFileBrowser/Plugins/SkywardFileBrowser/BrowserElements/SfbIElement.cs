namespace SkywardRay.FileBrowser {
	public interface SfbIElement {
		void Init (SfbInternal fileBrowser);

		void SetFocus ();

		void ReceiveMessage (string message);
	}
}