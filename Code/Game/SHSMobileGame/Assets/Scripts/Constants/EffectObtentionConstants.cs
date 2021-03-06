﻿using System;
using System.Collections.Generic;


public static class EffectObtentionConstants
	{
	public static Dictionary<string,int> achievementStatMaxValue = new Dictionary<string,int>{
		{ "numberOfTerminalPlaced", 10},
		{ "numberOfTerminalBuffed", 50},
		{ "numberOfTerminalDamaged", 50},
		{ "numberOfTerminalImproved", 50},
		{ "numberOfZoneHealed", 100},
		{ "numberOfZoneImproved", 10},
		};

	public static Dictionary<string,int> achievementSkinsMaxValue = new Dictionary<string,int>{
		{ "1SkinAchievement", 1},
		{ "4SkinAchievement", 4},
		{ "allSkinAchievement", 7}
	};

	public static Dictionary<string,int> medalNumberObtention = new Dictionary<string,int>{
		{ "numberOfTerminalPlaced", 1},
		{ "numberOfTerminalBuffed", 3},
		{ "numberOfTerminalDamaged", 2},
		{ "numberOfTerminalImproved", 2},
		{ "numberOfZoneHealed", 3},
		{ "numberOfZoneImproved", 1}
	};

	public static int numberOfAllAchivement = 11;
		
	}


