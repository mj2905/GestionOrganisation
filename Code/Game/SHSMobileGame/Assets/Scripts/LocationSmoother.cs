using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationSmoother : MonoBehaviour {

	public LocationListener[] listeners;

	/**
	 * Speed in meters/s
	 **/

	public float speed = 1;
	private bool firstLocation;
	private bool destinationReached;
	private XYCoordinate previousPosition;
	private XYCoordinate actualPosition;
	private XYCoordinate nextPosition;

	private float pathLength;
	private float pathPercent;

	void Start() {
		firstLocation = true;
		destinationReached = true;
		pathPercent = 0;
		pathLength = 1;
		previousPosition = XYCoordinate.ZERO ();
		actualPosition = XYCoordinate.ZERO ();
		nextPosition = XYCoordinate.ZERO ();
	}

	void Update() {
		if (!destinationReached) {
			
			pathPercent += Time.deltaTime * speed * pathLength / 25.0f;
			if (pathPercent > 1) {
				pathPercent = 1;
				destinationReached = true;
			}

			actualPosition = previousPosition + pathPercent * (nextPosition - previousPosition);

			foreach (LocationListener listener in listeners) {
				listener.CoordinateUpdate (actualPosition);
			}
		}
	}

	public void CoordinateUpdate(MapCoordinate coords) {

		nextPosition = coords.toXYMercator ();

		if (firstLocation) {
			destinationReached = true;
			actualPosition = nextPosition;

			foreach (LocationListener listener in listeners) {
				listener.CoordinateUpdate (actualPosition);
				listener.FirstLocationSent ();
			}
			firstLocation = false;

		} else {
			
			previousPosition = actualPosition;

			pathLength = previousPosition.distanceTo (nextPosition);
			if (pathLength > 10) {
				pathLength = Mathf.Log (pathLength);
			} else {
				pathLength = 2.3f;
			}
			pathPercent = 0;

			destinationReached = false;
		}

	}

	public void StopLocationHandling() {
		foreach (LocationListener listener in listeners) {
			listener.StopLocationHandling ();
		}
		firstLocation = true;
		destinationReached = true;
	}
}