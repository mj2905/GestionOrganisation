using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceCreditUpdateHandle : MonoBehaviour {

	public RectTransform parent;
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.localPosition;

		pos.x = parent.rect.size.x;
		transform.localPosition = pos;
	}
}
