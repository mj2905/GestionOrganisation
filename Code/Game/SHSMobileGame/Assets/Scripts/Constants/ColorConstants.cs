using UnityEngine;

public static class ColorConstants
{
	public static Color team1 = new Color(0,1,0,0.2f);
	public static Color team2 = new Color(1,0,0,0.2f);
	public static Color team3 = new Color(1,1,0,0.2f);
	public static Color team4 = new Color(0,0,1,0.2f);

	public static Color getColor(int teamNumber){
		Color color;
		switch (teamNumber) {
		case 1:
			color = ColorConstants.team1;
			break;
		case 2:
			color = ColorConstants.team2;
			break;
		case 3:
			color = ColorConstants.team3;
			break;
		case 4:
			color = ColorConstants.team4;
			break;
		default:
			color = ColorConstants.team1;
			break;
		}
		return color;
	}
}


