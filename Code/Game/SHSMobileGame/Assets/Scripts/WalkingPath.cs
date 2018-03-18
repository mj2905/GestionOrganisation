using System;
using UnityEngine.Assertions;

public class WalkingPath {

	private readonly double speed;
	private readonly MapCoordinate begPoint;
	private readonly MapCoordinate endPoint;
	private double lambda;
	private double direction = 1;
	public bool switchAllowed = false;

	public WalkingPath(MapCoordinate begPoint, MapCoordinate endPoint, double speed=1) {
		Assert.IsTrue (speed > 0 && speed < 10);

		this.begPoint = begPoint;
		this.endPoint = endPoint;
		this.speed = speed;
		this.lambda = 0;
	}

	public MapCoordinate next() {
		MapCoordinate next = (1-lambda) * begPoint + lambda * endPoint;
		lambda += direction*(speed / 1000);

		if (lambda > 1) {
			direction = switchAllowed ? -1 : 1;
			lambda = 1;
		} else if (lambda < 0) {
			direction = switchAllowed ? 1 : -1;
			lambda = 0;
		}

		return next;
	}
	
}
