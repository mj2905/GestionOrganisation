using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour {

    private bool visible;
    public GameObject menu;

	// Use this for initialization
	void Start () {
        visible = false;
	}
	
	public void toggleMenu()
    {
        visible = !visible;
        menu.SetActive(visible);
    }
}
