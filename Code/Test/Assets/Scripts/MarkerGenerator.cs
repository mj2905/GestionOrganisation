using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerGenerator : MonoBehaviour {


	[SerializeField]
	private Transform map;
	[SerializeField]
	private Transform prefab;

	[SerializeField]
	private float x;
	[SerializeField]
	private float y;

	[SerializeField]
	private float ul_x;
	[SerializeField]
	private float ul_y;

	[SerializeField]
	private float br_x;
	[SerializeField]
	private float br_y;

	private CoordinateMap coordinate;
	private Transform marker;

	// Use this for initialization
	void Start () {
		coordinate = new RealToMapCoordinate (x, y, new CoordinateBound (ul_x, ul_y), new CoordinateBound (br_x, br_y), map);
		marker = (Transform)Instantiate(prefab, coordinate.toMapVector(), Quaternion.identity);
	}

	// Update is called once per frame
	void Update () {
		try {
			marker.transform.position = new RealToMapCoordinate (x, y, new CoordinateBound (ul_x, ul_y), new CoordinateBound (br_x, br_y), map).toMapVector ();
		}
		catch (OutOfMapException e) {
			Debug.Log(e.Message);
		}
	}
}
