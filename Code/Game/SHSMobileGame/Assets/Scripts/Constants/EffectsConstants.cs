using System;


public class EffectsConstants
{
	public static Medal medal101 = new Medal("101",5,10);	
	public static Medal medal202 = new Medal("202",1,10);	
	public static Medal medal404 = new Medal("404",2,15);	
	public static Medal medal505 = new Medal("505",4,5);	

	public static Medal terminalPlacedMedal = new Medal("terminalPlacedMedal",3,5);
	public static Medal terminalDamagedMedal = new Medal("terminalDamagedMedal",3,5);	
	public static Medal terminalBuffedMedal = new Medal("terminalBuffedMedal",3,5);	
	public static Medal zoneHealedMedal = new Medal("zoneHealedMedal",3,5);

	public static Achievement terminalPlacedAchievement = new Achievement("terminalPlacedAchievement",3);
	public static Achievement terminalDamagedAchievement = new Achievement("terminalDamagedAchievement",3);	
	public static Achievement terminalBuffedAchievement = new Achievement("terminalBuffedAchievement",3);	
	public static Achievement zoneHealedAchievement = new Achievement("zoneHealedAchievement",3);	


	public static Medal GetMedalByName(string name){
		switch (name) {
		case "101":
			return medal101;
		case "202":
			return medal202;
		case "404":
			return medal404;
		case "505":
			return medal505;
		case "terminalPlacedMedal":
			return terminalPlacedMedal;
		case "terminalDamagedMedal":
			return terminalDamagedMedal;
		case "terminalBuffedMedal":
			return terminalBuffedMedal;
		case "zoneHealedMedal":
			return zoneHealedMedal;
		}

		return null;
	}
}

