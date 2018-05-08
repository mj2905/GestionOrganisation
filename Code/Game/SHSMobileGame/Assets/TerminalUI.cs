using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalUI : MonoBehaviour {

	private Quaternion rotation;

	void Start(){
		rotation = Random.rotationUniform;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -7) {
			transform.position = new Vector3(Random.Range (-3, 3),10 + Random.Range (0, 2), Random.Range (0, 800));
			float scale = Random.Range (0.5f, 1.5f);
			transform.localScale = Vector3.one * scale;
			transform.rotation = Random.rotationUniform;
			rotation = Random.rotationUniform;
		}


		transform.position = new Vector3 (transform.position.x,transform.position.y - Time.deltaTime * transform.localScale.x, transform.position.z);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, rotation, 0.1f);

		if (transform.rotation == rotation) {
			rotation = Random.rotationUniform;
		}
	}
}
