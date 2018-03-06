using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingPlayer : MonoBehaviour {

	public float fadingSpeed = 1;
	private const float factor = 0.01f;
	private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Renderer>().enabled = false;
		initialPosition = new Vector3(77.15f, 2, -50.0f);
		transform.localPosition = new Vector3(12, 2, -7);
	}

	private static float max(float a, float b) {
		return a > b ? a : b;
	}

	private void SetCoords(MapCoordinate coords) {
		XYCoordinate posXY = CoordinateConstants.EPFL_CENTER_DIF + coords.toXYMercator ();

		double moveHorizontal = (posXY.x() - CoordinateConstants.EPFL_TOP_LEFT_XY.x())*CoordinateConstants.H_FACTOR/CoordinateConstants.EPFL_HORIZONTAL_DISTANCE;
		double moveVertical = (posXY.y() - CoordinateConstants.EPFL_TOP_LEFT_XY.y())*CoordinateConstants.V_FACTOR/CoordinateConstants.EPFL_VERTICAL_DISTANCE;

		transform.localPosition = initialPosition - new Vector3 ((float)moveHorizontal, 0.0f, (float)moveVertical);
	}

	public void SetCoordsVisible(MapCoordinate coords) {
		gameObject.GetComponent<Renderer>().enabled = true;
		Color color = gameObject.GetComponent<MeshRenderer> ().material.color;
		color.a = 1;
		gameObject.GetComponent<MeshRenderer> ().material.color = color;

		SetCoords(coords);
	}

	public void SetCoordsInvisible(MapCoordinate coords) {

		gameObject.GetComponent<Renderer>().enabled = false;

		SetCoords(coords);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Color color = gameObject.GetComponent<MeshRenderer> ().material.color;
		color.a = max(color.a - factor * fadingSpeed, 0);
		gameObject.GetComponent<MeshRenderer> ().material.color = color;

		if(color.a == 0) {
			gameObject.GetComponent<Renderer>().enabled = false;
			transform.localPosition = new Vector3(12, 2, -7);
		}
	}
}
