using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningWheel : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		GetComponent<RectTransform>().Rotate(Vector3.back * Time.deltaTime*90);
	}
		
}
