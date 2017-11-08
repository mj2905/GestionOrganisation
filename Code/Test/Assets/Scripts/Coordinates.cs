using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class Range {
	abstract public float inf();
	abstract public float sup();
}

public class Unconstrained : Range {

	private static Unconstrained instance;

	private Unconstrained() {}

	public static Unconstrained get() {
		if (instance == null)
		{
			instance = new Unconstrained();
		}
		return instance;
	}


	override public float inf() {
		return -1;
	}

	override public float sup() {
		return -1;
	}

	public override string ToString ()
	{
		return "No constraint";
	}
}

public class RangeCoord : Range {
	private float liminf;
	private float limsup;

	public RangeCoord(float liminf, float limsup) {
		this.liminf = liminf;
		this.limsup = limsup;
	}

	override public float inf() {
		return liminf;
	}

	override public float sup() {
		return limsup;
	}

	public override string ToString ()
	{
		return "["+liminf+","+limsup+"]";
	}
}



public class OutOfMapException: ArgumentException {
	public OutOfMapException():base() { } 
	public OutOfMapException (string message): base(message) { }
}


public abstract class Coordinate {
	private float x_;
	private float y_;

	public Coordinate(float x_, float y_, Range xRange, Range yRange) {
		this.x_ = x_;
		this.y_ = y_;

		if ((xRange is RangeCoord && (x_ < xRange.inf() || x_ > xRange.sup()))
			|| (yRange is RangeCoord && (y_ < yRange.inf() || y_ > yRange.sup()))) {
			throw new OutOfMapException("Coordinates "+ this + " out of given range x: " + xRange + " , y: " + yRange + ".");
		}
	}

	public float x() {
		return x_;
	}

	public float y() {
		return y_;
	}

	public override string ToString ()
	{
		return "(x: "+x_+", y: "+y_+")";
	}

}

public class RealToMapCoordinate {

	private CoordinateReal realPoint; 
	private CoordinateMap mappedPoint;

	public RealToMapCoordinate(float x, float y, CoordinateBound upLeft, CoordinateBound botRight, Transform transform) {
		realPoint = new CoordinateReal (x, y, upLeft, botRight);
		mappedPoint = new CoordinateMap (realPoint.normalized (), transform);
	}

	public float x() {
		return realPoint.x();
	}

	public float y() {
		return realPoint.y();
	}

	public Vector3 toMapVector() {
		return mappedPoint.toMapVector ();
	}
}

public class CoordinateBound : Coordinate {
	public CoordinateBound(float x, float y) : base(x, y, Unconstrained.get(), Unconstrained.get()) {}
}

internal class CoordinateMap : Coordinate {

	private static CoordinateBound getUpLeft(Transform transform) {
		return new CoordinateBound (transform.position.x - transform.GetComponent<Renderer> ().bounds.size.x / 2, 
			transform.position.y - transform.GetComponent<Renderer> ().bounds.size.y / 2);
	}

	private static CoordinateBound getBotRight(Transform transform) {
		return new CoordinateBound (transform.position.x + transform.GetComponent<Renderer> ().bounds.size.x / 2, 
			transform.position.y + transform.GetComponent<Renderer> ().bounds.size.y / 2);
	}

	public CoordinateMap(CoordinateNormalized coord, Transform transform) : this(coord.x(), coord.y(), getUpLeft (transform), getBotRight (transform)) {}
	private CoordinateMap(float x, float y, Coordinate upLeft, Coordinate botRight) : 
		base(Mathf.Lerp (upLeft.x (), botRight.x (), x), 
			 	Mathf.Lerp (upLeft.y (), botRight.y (), y), 
			 	new RangeCoord(upLeft.x(), botRight.x()), 
				new RangeCoord(upLeft.y(), botRight.y())) {}

	public Vector3 toMapVector() {
		return new Vector3 (x(), y(), -1);
	}
}

internal class CoordinateNormalized : Coordinate {
	public CoordinateNormalized(float x, float y) : base(x, 1-y, new RangeCoord(0, 1), new RangeCoord(0, 1)) {}
}

internal class CoordinateReal : Coordinate {
	private CoordinateNormalized point;

	public CoordinateReal(float x, float y, CoordinateBound upLeft, CoordinateBound botRight) 
		: base(x, y, new RangeCoord(upLeft.x(), botRight.x()), new RangeCoord(upLeft.y(), botRight.y())) {

		float xMap = (x - upLeft.x ()) / (botRight.x () - upLeft.x ());
		float yMap = (y - upLeft.y ()) / (botRight.y () - upLeft.y ());

		point = new CoordinateNormalized (xMap, yMap);
	}

	public CoordinateNormalized normalized() {
		return point;
	}


}