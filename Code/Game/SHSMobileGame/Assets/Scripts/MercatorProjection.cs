using System;

/*
 * This code is taken from
 * Mercator, http://wiki.openstreetmap.org/w/index.php?title=Mercator&oldid=1531699 (last visited février 28, 2018)
 */

public static class MercatorProjection
{
	private static readonly double R_MAJOR = 6378137.0;
	private static readonly double DEG2RAD = Math.PI / 180.0;
	private static readonly double RAD2Deg = 180.0 / Math.PI;
	private static readonly double PI_4 = Math.PI / 4.0;

	private static double RadToDeg(double rad)
	{
		return rad * RAD2Deg;
	}

	private static double DegToRad(double deg)
	{
		return deg * DEG2RAD;
	}
		
	public static double latToY (double latitude)
	{
		return System.Math.Log(System.Math.Tan( DegToRad(latitude) / 2 + PI_4 )) * R_MAJOR;
	}

	public static double lonToX (double longitude)
	{
		return DegToRad(longitude) * R_MAJOR;
	}
}