using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects{
	public List<Achievement> achievements = new List<Achievement> ();
	public List<MedalInfo> medals = new List<MedalInfo> ();

	public Effects (){
	}

	public Effects (List<Achievement> achievements,List<MedalInfo> medals){
		this.medals = medals;
		this.achievements = achievements;
	}

	public Effects (System.Object effects)
	{
		if (effects != null) {
			IDictionary<string,System.Object> effectsDict = (IDictionary<string,System.Object>)effects;
			foreach (KeyValuePair<string, System.Object> entry in effectsDict) {
				IDictionary<string,System.Object> stat = (IDictionary<string,System.Object>)entry.Value;
				int ttl = Int32.Parse(stat["ttl"].ToString());
				int multiplier = Int32.Parse(stat["multiplier"].ToString());

				if (ttl == QuantitiesConstants.TTL_ACHIEVEMENT) {
					achievements.Add (new Achievement (entry.Key,multiplier));
				} else {
					medals.Add (new MedalInfo (entry.Key,multiplier,ttl));
				}
			}
		}
	}

	public List<MedalInfo> SortMedalsByTtl(){
		medals.Sort((MedalInfo x, MedalInfo y) => x.GetTtl().CompareTo(x.GetTtl()));
		return medals;
	}

	public int GetTotalMultiplier(){
		int res = 1;

		foreach (MedalInfo medal in medals) {
			res *= medal.GetMultiplier ();
		}
		foreach (Achievement achievement in achievements) {
			res *= achievement.GetMultiplier ();
		}
		return res;
	}

	private Effects GetDifferenceEffects(Effects previousEffects){
		HashSet<MedalInfo> medalsHash = new HashSet<MedalInfo> (this.medals);
		HashSet<MedalInfo> previousMedalsHash = new HashSet<MedalInfo> (previousEffects.medals);

		medalsHash.ExceptWith (previousMedalsHash);

		HashSet<Achievement> achievementsHash = new HashSet<Achievement> (this.achievements);
		HashSet<Achievement> previousAchievementsHash = new HashSet<Achievement> (previousEffects.achievements);

		achievementsHash.ExceptWith (previousAchievementsHash);

		return new Effects (new List<Achievement>(achievementsHash), new List<MedalInfo>(medalsHash));
	}

	public Effects GetNewEffects(Effects previousEffects){
		return this.GetDifferenceEffects (previousEffects);
	}

	public Effects GetDeletedEffects(Effects previousEffects){
		Effects e = previousEffects.GetDifferenceEffects (this);
		return previousEffects.GetDifferenceEffects (this);
	}

	public Effects GetModifiedEffects(Effects previousEffects){
		HashSet<MedalInfo> medalsHash = new HashSet<MedalInfo> (this.medals);
		HashSet<MedalInfo> previousMedalsHash = new HashSet<MedalInfo> (previousEffects.medals);
				
		medalsHash.IntersectWith (previousMedalsHash);

		HashSet<Achievement> achievementsHash = new HashSet<Achievement> (this.achievements);
		HashSet<Achievement> previousAchievementsHash = new HashSet<Achievement> (previousEffects.achievements);

		achievementsHash.IntersectWith (previousAchievementsHash);

		return new Effects (new List<Achievement>(achievementsHash), new List<MedalInfo>(medalsHash));
	}
		
}

