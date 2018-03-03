public static class CoordinateConstants {

	public const bool DEBUG = true;

	public const double TEST_LOC_LAT = 46.55598, TEST_LOC_LONG = 6.699792;
	public const double ROLEX_LAT = 46.5189, ROLEX_LONG = 6.5683;
	public const double EPFL_CENTER_LAT = 46.52018, EPFL_CENTER_LONG = 6.56586;
	public const double EPFL_TOP_LEFT_LAT = 46.52261, EPFL_TOP_LEFT_LONG = 6.56058;
	public const double EPFL_TOP_RIGHT_LAT = 46.52261, EPFL_TOP_RIGHT_LONG = 6.5731;
	public const double EPFL_BOT_LEFT_LAT = 46.51705, EPFL_BOT_LEFT_LONG = 6.56058;
	public const double EPFL_BOT_RIGHT_LAT = 46.51705, EPFL_BOT_RIGHT_LONG = 6.5731;

	public static readonly double TEST_LOC_POS_Y = MercatorProjection.latToY(TEST_LOC_LAT), TEST_LOC_POS_X = MercatorProjection.lonToX(TEST_LOC_LONG);
	public static readonly double ROLEX_POS_Y = MercatorProjection.latToY(ROLEX_LAT), ROLEX_POS_X = MercatorProjection.lonToX(ROLEX_LONG);
	public static readonly double EPFL_CENTER_POS_Y = MercatorProjection.latToY(EPFL_CENTER_LAT), EPFL_CENTER_POS_X = MercatorProjection.lonToX(EPFL_CENTER_LONG);

	public static readonly double EPFL_TOP_LEFT_POS_Y = MercatorProjection.latToY(EPFL_TOP_LEFT_LAT), EPFL_TOP_LEFT_POS_X = MercatorProjection.lonToX(EPFL_TOP_LEFT_LONG);
	public static readonly double EPFL_TOP_RIGHT_POS_Y = MercatorProjection.latToY(EPFL_TOP_RIGHT_LAT), EPFL_TOP_RIGHT_POS_X = MercatorProjection.lonToX(EPFL_TOP_RIGHT_LONG);
	public static readonly double EPFL_BOT_LEFT_POS_Y = MercatorProjection.latToY(EPFL_BOT_LEFT_LAT), EPFL_BOT_LEFT_POS_X = MercatorProjection.lonToX(EPFL_BOT_LEFT_LONG);
	public static readonly double EPFL_BOT_RIGHT_POS_Y = MercatorProjection.latToY(EPFL_BOT_RIGHT_LAT), EPFL_BOT_RIGHT_POS_X = MercatorProjection.lonToX(EPFL_BOT_RIGHT_LONG);

	public static readonly double EPFL_HORIZONTAL_DISTANCE = EPFL_TOP_RIGHT_POS_X - EPFL_TOP_LEFT_POS_X;
	public static readonly double EPFL_VERTICAL_DISTANCE = EPFL_TOP_LEFT_POS_Y - EPFL_BOT_LEFT_POS_Y;


}
