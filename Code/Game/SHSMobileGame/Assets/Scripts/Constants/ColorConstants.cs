using UnityEngine;

public static class ColorConstants
{

	public enum TEAMS {ENAC = 1, STI = 2, FSB = 3, ICSV = 4};
	public static Color team1 = new Color(0,1,0,0.2f); // Green
	public static Color team2 = new Color(1,0,0,0.2f); // Red
	public static Color team3 = new Color(1,1,0,0.2f); // Yellow 
	public static Color team4 = new Color(0,0,1,0.2f); // Blue

	public static Color GRAY = new Color(167.0f/255.0f, 167.0f/255.0f, 167.0f/255.0f, 1.0f);

	public static Color attack = new Color(0.12f,0.12f,0.12f,0.2f); // Black

	public static string textTeam1 = "ENAC";
	public static string textTeam2 = "STI";
	public static string textTeam3 = "FSB";
	public static string textTeam4 = "IC&SV";

	public static Color textColorTeam1 = new Color(50/255.0f, 255/255.0f, 50/255.0f, 1.0f); // Green
	public static Color textColorTeam2 = new Color(255/255.0f, 81/255.0f, 81/255.0f, 1.0f); // Red
	public static Color textColorTeam3 = new Color(255/255.0f, 255/255.0f, 0/255.0f, 1.0f); // Yellow
	public static Color textColorTeam4 = new Color(94/255.0f, 182/255.0f, 255/255.0f, 1.0f); // Blue
	public static Color textColorDefault = new Color(1.0f,1.0f,1.0f, 1.0f); // Blue

	public static Color colorNotificationAlly = new Color(50/255.0f, 50/255.0f, 220/255.0f, 0.8f); // Green
	public static Color colorNotificationEnemy = new Color(220/255.0f, 50/255.0f, 50/255.0f, 0.8f); // Green

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

	public static Color getTextColor(int teamNumber){
		Color color;
		switch (teamNumber) {
		case 1:
			color = ColorConstants.textColorTeam1;
			break;
		case 2:
			color = ColorConstants.textColorTeam2;
			break;
		case 3:
			color = ColorConstants.textColorTeam3;
			break;
		case 4:
			color = ColorConstants.textColorTeam4;
			break;
		default:
			color = ColorConstants.textColorDefault;
			break;
		}
		return color;
	}

	public static string getColorAsString(int teamNumber){

		string color;
		switch (teamNumber) {
		case 1:
			color = ColorConstants.textTeam1;
			break;
		case 2:
			color = ColorConstants.textTeam2;
			break;
		case 3:
			color = ColorConstants.textTeam3;
			break;
		case 4:
			color = ColorConstants.textTeam4;
			break;
		default:
			color = ColorConstants.textTeam1;
			break;
		}
		return color;
	}
}


