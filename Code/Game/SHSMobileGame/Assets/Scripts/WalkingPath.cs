using System;
using UnityEngine.Assertions;

public class WalkingPath {

	private readonly double speed;
	private readonly MapCoordinate begPoint;
	private readonly MapCoordinate endPoint;
	private double lambda;

	public WalkingPath(MapCoordinate begPoint, MapCoordinate endPoint, double speed=1) {
		Assert.IsTrue (speed > 0 && speed < 10);

		this.begPoint = begPoint;
		this.endPoint = endPoint;
		this.speed = speed;
		this.lambda = 0;
	}

	public MapCoordinate next() {
		MapCoordinate next = lambda * begPoint + (1 - lambda) * endPoint;
		lambda += speed / 100;
		return next;
	}
	
}
