using System;


public class EffectsConstants
{
	public static MedalInfo medal101 = new MedalInfo("101",5,10);	
	public static MedalInfo medal202 = new MedalInfo("202",1,10);	
	public static MedalInfo medal404 = new MedalInfo("404",2,15);	
	public static MedalInfo medal505 = new MedalInfo("505",4,5);	

	public static MedalInfo terminalPlacedMedal = new MedalInfo("terminalPlacedMedal",3,5);
	public static MedalInfo terminalDamagedMedal = new MedalInfo("terminalDamagedMedal",3,5);	
	public static MedalInfo terminalBuffedMedal = new MedalInfo("terminalBuffedMedal",3,5);	
	public static MedalInfo zoneHealedMedal = new MedalInfo("zoneHealedMedal",3,5);

	public static Achievement terminalPlacedAchievement = new Achievement("terminalPlacedAchievement",3);
	public static Achievement terminalDamagedAchievement = new Achievement("terminalDamagedAchievement",3);	
	public static Achievement terminalBuffedAchievement = new Achievement("terminalBuffedAchievement",3);	
	public static Achievement terminalImprovedAchievement = new Achievement("terminalImprovedAchievement",3);	
	public static Achievement zoneHealedAchievement = new Achievement("zoneHealedAchievement",3);
	public static Achievement zoneImprovedAchievement = new Achievement("zoneImprovedAchievement",3);

	public static MedalInfo GetMedalByName(string name){
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

