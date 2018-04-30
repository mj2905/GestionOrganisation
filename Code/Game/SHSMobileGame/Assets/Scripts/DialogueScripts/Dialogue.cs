using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Host all information for a single dialogue

[System.Serializable]
public class Dialogue {

	public string name;
	[TextArea(3,5)]
	public string[] sentences;
}
