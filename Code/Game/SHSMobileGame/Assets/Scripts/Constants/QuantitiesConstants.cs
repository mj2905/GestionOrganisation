﻿
public static class QuantitiesConstants
{
	public const int TERMINAL_MAX_HEALTH = 100;
	public const int TERMINAL_MIN_HEALTH = 0;

	public const int ZONE_MIN_HEALTH = 0;

	public const int TERMINAL_IMPROVE_COST = -100;

	public const int TERMINAL_SMASH_AMOUNT = 20;
	public const int TERMINAL_SMASH_COST = -TERMINAL_SMASH_AMOUNT;
	public const int TERMINAL_BUFF_AMOUNT = 20;
	public const int TERMINAL_BUFF_COST = -TERMINAL_BUFF_AMOUNT;
	public const int ZONE_HEAL_AMOUNT = 20;
	public const int ZONE_HEAL_COST = -ZONE_HEAL_AMOUNT;
	public const int MINIMUM_HEALTH_SWITCH_COLOR = 201;

	public static readonly int[] ZONE_MAX_HEALTH_VALUES = new int[] {1000, 1500, 2000, 3000, 5000, 10000};
	public static readonly int[] ZONE_MAX_HEALTH_COST = new int[] {0, 3000, 4000, 6000, 10000, 20000};


	public static readonly int[] TERMINAL_MAX_HEALTH_VALUES = new int[] {100, 200, 500, 1000, 5000};
	public static readonly int[] TERMINAL_MAX_HEALTH_COST = new int[] {0, 3000, 4000, 6000, 10000, 20000};

	public static readonly int[] PLAYER_XP_THRESHOLDS = new int[]{100,500,2000,7500,20000};

	public const int STRENGTH_MAX = 20;
	public static readonly int TERMINAL_LEVEL_MAX = TERMINAL_MAX_HEALTH_VALUES.Length - 1;
	public static readonly int ZONE_LEVEL_MAX = ZONE_MAX_HEALTH_VALUES.Length - 1;
	public const int TTL_ACHIEVEMENT = -100;
}


