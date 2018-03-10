using System;
using System.Collections;
using System.Collections.Generic;

public class Effects{
	public List<Achievement> achievements = new List<Achievement> ();
	public List<Medal> medals = new List<Medal> ();

	public Effects (){}

	public Effects (System.Object effects)
	{
		if (effects != null) {
			IDictionary<string,System.Object> effectsDict = (IDictionary<string,System.Object>)effects;
			foreach (KeyValuePair<string, System.Object> entry in effectsDict) {
				IDictionary<string,System.Object> stat = (IDictionary<string,System.Object>)entry.Value;
				int ttl = Int32.Parse(stat["ttl"].ToString());
				int multiplier = Int32.Parse(stat["multiplier"].ToString());

				if (ttl == QuantitiesConstants.TTL_ACHIEVEMENT) {
					achievements.Add (new Achievement (multiplier,entry.Key));
				} else {
					medals.Add (new Medal (multiplier,ttl));
				}
			}
		}
	}

	public List<Medal> SortMedalsByTtl(){
		medals.Sort((Medal x, Medal y) => x.GetTtl().CompareTo(x.GetTtl()));
		return medals;
	}

	public int GetTotalMultiplier(){
		int res = 1;

		foreach (Medal medal in medals) {
			res *= medal.GetMultiplier ();
		}
		foreach (Achievement achievement in achievements) {
			res *= achievement.GetMultiplier ();
		}
		return res;
	}
}

