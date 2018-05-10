using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLinks : MonoBehaviour {

	public void twitter() {
		Application.OpenURL("https://twitter.com/ClashoFaculties");
	}

	public void facebook() {
		Application.OpenURL("https://www.facebook.com/ClashofFaculties");
	}

	public void website() {
		Application.OpenURL("https://gom-industries.blogspot.com/");
	}

}
