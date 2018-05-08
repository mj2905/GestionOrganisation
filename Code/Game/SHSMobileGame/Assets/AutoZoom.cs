using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoZoom : MonoBehaviour {

	public Camera mainCamera;
	public Camera secondaryCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		secondaryCamera.fieldOfView = mainCamera.fieldOfView;
	}
}
