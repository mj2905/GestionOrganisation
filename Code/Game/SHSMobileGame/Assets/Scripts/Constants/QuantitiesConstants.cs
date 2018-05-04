using System.Collections.Generic;
using System;

public static class QuantitiesConstants
{
	public const int TERMINAL_MAX_HEALTH = 100;
	public const int TERMINAL_MIN_HEALTH = 0;

	public const int ZONE_MIN_HEALTH = 0;

	public const int TERMINAL_IMPROVE_COST = -100;

	public const int TERMINAL_SMASH_AMOUNT = 20;
	public const int TERMINAL_SMASH_COST = -TERMINAL_SMASH_AMOUNT;

	public static readonly int[] TERMINAL_SMASH_LEVEL_BONUS = new int[] {0, 5, 10, 15, 20, 25};
	public static readonly int[] ZONE_HEAL_LEVEL_BONUS = new int[] {0, 10, 20, 30, 35, 40};

	public const int ZONE_HEAL_AMOUNT = 30;
	public const int ZONE_HEAL_COST = -15;
	public const int MINIMUM_HEALTH_SWITCH_COLOR = 201;

	public static readonly int[] ZONE_MAX_HEALTH_VALUES = new int[] {1000, 1500, 2000, 3000, 5000, 10000};
	public static readonly int[] ZONE_MAX_HEALTH_COST = new int[] {0, 3000, 4000, 6000, 10000, 20000};

	private static readonly int[] TERMINAL_BUFF_COST = new int[] {0, 100, 150, 200, 250, 350};
	private static readonly int[] TERMINAL_BUFF_VALUE = new int[] {30, 50, 65, 80, 95, 120};

	public static int getTerminalBuffCost(int actualStrength) {
		int k = Array.IndexOf(TERMINAL_BUFF_VALUE, actualStrength);
		return TERMINAL_BUFF_COST [k+1];
	}

	public static int getTerminalBuffNext(int actualStrength) {
		int k = Array.IndexOf(TERMINAL_BUFF_VALUE, actualStrength);
		return TERMINAL_BUFF_VALUE [k+1];
	}

	public static readonly int[] TERMINAL_MAX_HEALTH_VALUES = new int[] {100, 150, 200, 250, 350};
	public static readonly int[] TERMINAL_MAX_HEALTH_COST = new int[] {0, 100, 150, 200, 250, 350};

	public static readonly int[] PLAYER_XP_THRESHOLDS = new int[]{0,100,500,2000,7500,20000};

	public static readonly int STRENGTH_MAX = TERMINAL_BUFF_VALUE[TERMINAL_BUFF_VALUE.Length - 2];
	public static readonly int TERMINAL_LEVEL_MAX = TERMINAL_MAX_HEALTH_VALUES.Length - 1;
	public static readonly int ZONE_LEVEL_MAX = ZONE_MAX_HEALTH_VALUES.Length - 1;
	public const int TTL_ACHIEVEMENT = -100;

	public static float BUFF_COOLDOWN = 2f;
	public static float SMASH_COOLDOWN = 2f;
	public static float HEAL_COOLDOWN = 2f;

	public static float ZONE_IMPROVE_COOLDOWN = 2f;
	public static float TERMINAL_IMPROVE_COOLDOWN = 2f;

	public static Dictionary<string, string> nameAchievementDisplay = new Dictionary<string, string>() {
		{ "zoneImprovedAchievement", "The wall just got ten feet higher" },
		{ "zoneHealedAchievement", "Meeedic"},
		{ "terminalImprovedAchievement", "Harder better faster stronger"},
		{ "terminalDamagedAchievement", "Not in my zone"},
		{ "terminalBuffedAchievement", "King of pew pew"},
		{ "terminalPlacedAchievement", "U kno da wae"},
		{ "4SkinAchievement", "Ready for Winter"},
		{ "allSkinAchievement", "Skin Changer"},
		{ "top5PlayerAchievement", "Not bad"},
		{ "bestPlayerAchievement","You're the best"},
		{ "allAchievement","GG WP"}
	};
}


