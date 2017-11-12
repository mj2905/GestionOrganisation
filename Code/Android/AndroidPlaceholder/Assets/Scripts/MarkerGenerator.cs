using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerGenerator : MonoBehaviour {


	[SerializeField]
	private Transform map;
	[SerializeField]
	private Transform prefab;
	[SerializeField]
	private Transform camera;

	public float x;// = -122.5f;
	public float y;// = 37.5f;

	[SerializeField]
	private float ul_x;
	[SerializeField]
	private float ul_y;

	[SerializeField]
	private float br_x;
	[SerializeField]
	private float br_y;

	private RealToMapCoordinate coordinate;
	private Transform marker;
	private Vector3 pos;

	// Use this for initialization
	void Start () {
		coordinate = new RealToMapCoordinate (x, y, new CoordinateBound (ul_x, ul_y), new CoordinateBound (br_x, br_y), map);
		marker = (Transform)Instantiate(prefab, coordinate.toMapVector(), Quaternion.identity);
	}

	// Update is called once per frame
	void Update () {
		try {
			pos = new RealToMapCoordinate (x, y, new CoordinateBound (ul_x, ul_y), new CoordinateBound (br_x, br_y), map).toMapVector ();
		}
		catch (OutOfMapException e) {
			Debug.Log(e.Message);
		}

		marker.transform.position = pos;
		camera.transform.position = pos;
	}
}
