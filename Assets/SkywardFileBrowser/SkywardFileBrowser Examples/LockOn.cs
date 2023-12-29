using UnityEngine;

namespace SkywardRay.FileBrowser.Example {
	public class LockOn : MonoBehaviour {
		public FirstPersonController player;

		private bool locked = false;

		// Use this for initialization
		private void Start () {}

		// Update is called once per frame
		private void Update () {
			if (Input.GetKeyUp(KeyCode.E)) {
				locked = !locked;

				if (locked) {
					player.enabled = false;
				}
				else {
					player.enabled = true;
				}
			}
		}
	}
}
