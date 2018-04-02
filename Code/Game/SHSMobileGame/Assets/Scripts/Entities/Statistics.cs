using System;
using System.Collections.Generic;

public class Statistics{

	private IDictionary<string,int> statisticsDict = new Dictionary<string,int>();

	public Statistics (System.Object statistics){
		InitStat ();

		if (statistics != null) {
			IDictionary<string,System.Object> objectDict = (IDictionary<string,System.Object>)statistics;
			foreach (KeyValuePair<string, System.Object> entry in objectDict) {
				statisticsDict [entry.Key] = Int32.Parse (entry.Value.ToString ());
			}
		}
	}

	public IDictionary<string,int> GetDict(){
		return statisticsDict;
	}

	private void InitStat(){
		foreach (KeyValuePair<string, int> entry in EffectObtentionConstants.achievementMaxValue) {
			statisticsDict [entry.Key] = 0;
		}
	}


	public Statistics (int numberOfTerminalPlaced, int numberOfTerminalBuffed, int numberOfTerminalDamaged, int numberOfZoneHealed)
	{
		statisticsDict ["numberOfTerminalPlaced"] = numberOfTerminalPlaced;
		statisticsDict ["numberOfTerminalBuffed"] = numberOfTerminalBuffed;
		statisticsDict ["numberOfTerminalDamaged"] = numberOfTerminalDamaged;
		statisticsDict ["numberOfZoneHealed"] = numberOfZoneHealed;
	}
}


