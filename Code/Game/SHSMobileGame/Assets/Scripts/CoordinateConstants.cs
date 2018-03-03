using System;

public static class CoordinateConstants {

	public const bool DEBUG = false;


	public static readonly MapCoordinate TEST_LOC_MAP = new MapCoordinate(6.699792, 46.55598);
	public static readonly MapCoordinate ROLEX_MAP = new MapCoordinate(6.5683, 46.5189);
	public static readonly MapCoordinate EPFL_CENTER_MAP = new MapCoordinate(6.56586, 46.52018);
	public static readonly MapCoordinate EPFL_TOP_LEFT_MAP = new MapCoordinate(6.56058, 46.52261);
	public static readonly MapCoordinate EPFL_TOP_RIGHT_MAP = new MapCoordinate(6.5731, 46.52261);
	public static readonly MapCoordinate EPFL_BOT_LEFT_MAP = new MapCoordinate(6.56058, 46.51705);
	public static readonly MapCoordinate EPFL_BOT_RIGHT_MAP = new MapCoordinate(6.5731, 46.51705);

	public static readonly XYCoordinate TEST_LOC_XY = TEST_LOC_MAP.toXYMercator();
	public static readonly XYCoordinate ROLEX_XY = ROLEX_MAP.toXYMercator();
	public static readonly XYCoordinate EPFL_CENTER_XY = EPFL_CENTER_MAP.toXYMercator();

	public static readonly XYCoordinate EPFL_TOP_LEFT_XY = EPFL_TOP_LEFT_MAP.toXYMercator();
	public static readonly XYCoordinate EPFL_TOP_RIGHT_XY = EPFL_TOP_RIGHT_MAP.toXYMercator();
	public static readonly XYCoordinate EPFL_BOT_LEFT_XY = EPFL_BOT_LEFT_MAP.toXYMercator();
	public static readonly XYCoordinate EPFL_BOT_RIGHT_XY = EPFL_BOT_RIGHT_MAP.toXYMercator();

	public static readonly double EPFL_HORIZONTAL_DISTANCE = EPFL_TOP_RIGHT_XY.x() - EPFL_TOP_LEFT_XY.x();
	public static readonly double EPFL_VERTICAL_DISTANCE = EPFL_TOP_LEFT_XY.y() - EPFL_BOT_LEFT_XY.y();


}

public class XYCoordinate : Tuple<double, double> {
	public XYCoordinate(double x, double y):base(x, y)
	{}

	private double Item1() { return base.Item1; }
	private double Item2() { return base.Item2; }

	public double x() {
		return Item1();
	}

	public double y() {
		return Item2();
	}

	public static XYCoordinate operator- (XYCoordinate a, XYCoordinate b) {
		return new XYCoordinate (a.x () - b.x (), a.y () - b.y ());
	}

	public static XYCoordinate operator+ (XYCoordinate a, XYCoordinate b) {
		return new XYCoordinate (a.x () + b.x (), a.y () + b.y ());
	}

	public static XYCoordinate ZERO() {
		return new XYCoordinate (0, 0);
	}

	public override string ToString ()
	{
		return "x:"+x() + "|y:"+y();
	}
}


public class MapCoordinate : Tuple<double, double> {
	public MapCoordinate(double lon, double lat):base(lon, lat)
	{}

	private double Item1() { return base.Item1; }
	private double Item2() { return base.Item2; }

	public double lon() {
		return Item1();
	}

	public double lat() {
		return Item2();
	}

	public XYCoordinate toXYMercator() {
		return new XYCoordinate (MercatorProjection.lonToX (lon ()), MercatorProjection.latToY (lat ()));
	}

	public static MapCoordinate ZERO() {
		return new MapCoordinate (0, 0);
	}

	public static MapCoordinate operator+ (MapCoordinate a, MapCoordinate b) {
		return new MapCoordinate(a.lon () + b.lon (), a.lat () + b.lat ());
	}

	public static bool operator< (MapCoordinate a, MapCoordinate b) {
		return a.lon () < b.lon () || a.lat () < b.lat ();
	}

	public static bool operator> (MapCoordinate a, MapCoordinate b) {
		return a.lon () > b.lon () || a.lat () > b.lat ();
	}

	public static bool operator== (MapCoordinate a, MapCoordinate b) {
		return a.lon () == b.lon() && a.lat() == b.lat();
	}

	public static bool operator!= (MapCoordinate a, MapCoordinate b) {
		return a.lon () != b.lon() || a.lat() != b.lat();
	}

	public static MapCoordinate operator* (double lambda, MapCoordinate a) {
		return new MapCoordinate(lambda * a.lon(), lambda * a.lat());
	}

	public override string ToString ()
	{
		return "lon:"+lon() + "|lat:"+lat();
	}
}