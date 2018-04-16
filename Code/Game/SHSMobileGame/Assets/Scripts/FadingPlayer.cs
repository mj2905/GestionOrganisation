using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadingPlayer : LocationListener
{

	public CameraController camera;
	public float speed;
	private GameObject currentZone;

	public Button modeButton;
	private string previousMessage = "Switch to Attack";

	private double moveVertical;
	private double moveHorizontal;

	//Obtained by computing the unity size of the map
	/*
	 * (77.15) x (50)
	957.9 m x 618.2 m
	0,01252 x 0,00556

	0.01252 -> 77.15
	x -> y

	y = 77.15x/0.01252
	 */

	private Vector3 initialPosition;

	void Start()
	{
		setVisible (false);
		initialPosition = new Vector3(77.15f, 2, -50.0f);
		transform.localPosition = initialPosition;

		moveVertical = 0;
		moveHorizontal = 0;

		currentZone = null;
	}

	Vector3 GetPosition() {
		return -new Vector3 ((float)moveHorizontal, 0.0f, (float)moveVertical);
	}

	void FixedUpdate()
	{
		transform.localPosition = initialPosition + GetPosition();
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "SafeZone") {
			currentZone = other.gameObject;
		} else {
			print ("Enter: " + other.gameObject.name);

			Transform p = other.transform;

			while (p != null && !p.name.EndsWith ("_building", System.StringComparison.CurrentCultureIgnoreCase)) {
				p = p.parent;
			}

			if (p != null) {
				Color color = p.GetComponent<MeshRenderer> ().material.color;
				color.a = 0.2f;
				p.GetComponent<MeshRenderer> ().material.color = color;
			} else {
				print ("No such parent");
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "SafeZone") {
			currentZone = null;
		} else {

			print ("Exited: " + other.gameObject.name);
			Transform p = other.transform;

			while (p != null && !p.name.EndsWith ("_building", System.StringComparison.CurrentCultureIgnoreCase)) {
				p = p.parent;
			}

			if (p != null) {
				Color color = p.GetComponent<MeshRenderer> ().material.color;
				color.a = 1;
				p.GetComponent<MeshRenderer> ().material.color = color;
			} else {
				print ("No such parent");
			}
		}
	}

	public void setVisible(bool visible) {
		gameObject.GetComponent<Renderer>().enabled = visible;
	}

	public bool isInsideSafeZone(){
		return currentZone != null;
	}

	override public void CoordinateUpdate(XYCoordinate coords) {

		double lastMovH = moveHorizontal, lastMovV = moveVertical;

		XYCoordinate posXY = CoordinateConstants.EPFL_CENTER_DIF + coords;

		moveHorizontal = (posXY.x() - CoordinateConstants.EPFL_TOP_LEFT_XY.x())*CoordinateConstants.H_FACTOR/CoordinateConstants.EPFL_HORIZONTAL_DISTANCE;
		moveVertical = (posXY.y() - CoordinateConstants.EPFL_TOP_LEFT_XY.y())*CoordinateConstants.V_FACTOR/CoordinateConstants.EPFL_VERTICAL_DISTANCE;

		/*
		if (CoordinateConstants.DEBUG != CoordinateConstants.DEBUG_STATE.NO_DEBUG) {
			if (moveHorizontal != lastMovH || moveVertical != lastMovV) {
				print ("mh:" + moveHorizontal + " | mv:" + moveVertical);
			}
		}
		*/

	}

	override public void StopLocationHandling() {
		modeButton.GetComponentInChildren<Text>().text = previousMessage;
		camera.LeaveFadingPlayer ();
		moveVertical = 0;
		moveHorizontal = 0;
		setVisible (false);
	}

	override public void FirstLocationSent() {
		setVisible (true);
		camera.GoToFadingPlayer ();
		previousMessage = modeButton.GetComponentInChildren<Text> ().text;
		modeButton.GetComponentInChildren<Text>().text = "Go to a safe Zone ! (Click to cancel)";
	}

}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingPlayer : MonoBehaviour {

	public CameraController camera;
	public float fadingSpeed = 1;
	private const float factor = 0.005f;
	private Vector3 initialPosition;
	private bool hasBeenVisible;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		initialPosition = new Vector3(77.15f, 2, -50.0f);
		transform.localPosition = initialPosition;
		hasBeenVisible = false;
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other is CapsuleCollider) {
			//print ("Safe zone entered: " + other.gameObject.name);
		}
		else {

			Transform p = other.gameObject.transform;

			while (!p.parent.name.EndsWith ("_building")) {
				p = p.parent;
			}

			other.gameObject.transform.position += new Vector3 (0, 1, 0);
			p.parent.position -= new Vector3 (0, 1, 0);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other is CapsuleCollider) {
			//print ("Safe zone exited: " + other.gameObject.name);
		} else {
			print ("Exited: " + other.gameObject.name);

			Transform p = other.gameObject.transform;

			while (!p.parent.name.EndsWith ("_building")) {
				p = p.parent;
			}

			p.parent.position += new Vector3 (0, 1, 0);
			other.gameObject.transform.position -= new Vector3 (0, 1, 0);

		}
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
		gameObject.GetComponent<MeshRenderer>().enabled = true;
		Color color = gameObject.GetComponent<MeshRenderer> ().material.color;
		color.a = 1;
		gameObject.GetComponent<MeshRenderer> ().material.color = color;

		camera.GoToFadingPlayer ();
		hasBeenVisible = true;
	}

	public void SetCoordsInvisible(MapCoordinate coords) {

		gameObject.GetComponent<MeshRenderer>().enabled = false;

		SetCoords(coords);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Color color = gameObject.GetComponent<MeshRenderer> ().material.color;
		color.a = max(color.a - factor * fadingSpeed, 0);
		gameObject.GetComponent<MeshRenderer> ().material.color = color;

		if(color.a == 0) {
			gameObject.GetComponent<Renderer>().enabled = false;
			if (hasBeenVisible) {
				camera.LeaveFadingPlayer ();
				hasBeenVisible = false;
			}
			transform.localPosition = initialPosition;
		}
	}
}
*/