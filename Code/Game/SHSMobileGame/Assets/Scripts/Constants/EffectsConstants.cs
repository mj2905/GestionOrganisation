using System;


public class EffectsConstants
{
	public static MedalInfo terminalPlacedMedal = new MedalInfo("terminalPlacedMedal",10,1);
	public static MedalInfo terminalDamagedMedal = new MedalInfo("terminalDamagedMedal",2,3);	
	public static MedalInfo terminalBuffedMedal = new MedalInfo("terminalBuffedMedal",4,2);	
	public static MedalInfo terminalImprovedMedal = new MedalInfo("terminalImprovedMedal",4,2);	
	public static MedalInfo zoneHealedMedal = new MedalInfo("zoneHealedMedal",2,3);
	public static MedalInfo zoneImprovedMedal = new MedalInfo("zoneImprovedMedal",10,1);


	public static Achievement terminalPlacedAchievement = new Achievement("terminalPlacedAchievement",3);
	public static Achievement terminalDamagedAchievement = new Achievement("terminalDamagedAchievement",3);	
	public static Achievement terminalBuffedAchievement = new Achievement("terminalBuffedAchievement",3);	
	public static Achievement terminalImprovedAchievement = new Achievement("terminalImprovedAchievement",3);	
	public static Achievement zoneHealedAchievement = new Achievement("zoneHealedAchievement",3);
	public static Achievement zoneImprovedAchievement = new Achievement("zoneImprovedAchievement",3);
	public static Achievement fourSkinAchievement = new Achievement("4SkinAchievement",3);
	public static Achievement allSkinAchievement = new Achievement("allSkinAchievement",3);	
	public static Achievement top5PlayerAchievement = new Achievement("top5PlayerAchievement",3);
	public static Achievement bestPlayerAchievement = new Achievement("bestPlayerAchievement",3);
	public static Achievement allAchievement = new Achievement("allAchievement",3);

	public static int terminalPlacedXp = 50;
	public static int terminalDamagedXp = 5;	
	public static int terminalBuffedXp = 10;	
	public static int terminalImprovedXp = 10;	
	public static int zoneHealedXp = 5;
	public static int zoneImprovedXp = 40;

	public static int terminalPlacedAchievementXp = 200;
	public static int terminalDamagedAchievementXp = 200;	
	public static int terminalBuffedAchievementXp = 200;	
	public static int terminalImprovedAchievementXp = 200;	
	public static int zoneHealedAchievementXp = 200;
	public static int zoneImprovedAchievementXp = 200;
	public static int fourSkinAchievementXp = 200;
	public static int allSkinAchievementXp = 400;	
	public static int top5PlayerAchievementXp = 200;
	public static int bestPlayerAchievementXp = 400;
	public static int allAchievementXp = 500;

	public static MedalInfo GetMedalByName(string name){
		switch (name) {
		case "terminalPlacedMedal":
			return terminalPlacedMedal;
		case "terminalDamagedMedal":
			return terminalDamagedMedal;
		case "terminalBuffedMedal":
			return terminalBuffedMedal;
		case "terminalImprovedMedal":
			return terminalImprovedMedal;
		case "zoneHealedMedal":
			return zoneHealedMedal;
		case "zoneImprovedMedal":
			return zoneImprovedMedal;
		}

		return null;
	}
}

