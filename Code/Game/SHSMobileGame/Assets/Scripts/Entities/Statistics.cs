using System;
using System.Collections.Generic;

public class Statistics{

	private IDictionary<string,int> statisticsDict = new Dictionary<string,int>();

	public Statistics (System.Object statistics){
		InitStat ();

		if (statistics != null) {
			IDictionary<string,System.Object> objectDict = (IDictionary<string,System.Object>)statistics;
			foreach (KeyValuePair<string, System.Object> entry in objectDict) {
				switch (entry.Key) {
				case "numberOfTerminalPlaced":
					statisticsDict ["numberOfTerminalPlaced"] = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfTerminalBuffed":
					statisticsDict ["numberOfTerminalBuffed"] = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfTerminalDamaged":
					statisticsDict ["numberOfTerminalDamaged"] = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfZoneHealed":
					statisticsDict ["numberOfZoneHealed"] = Int32.Parse (entry.Value.ToString ());
					break;
				}
			}
		}
	}

	public IDictionary<string,int> GetDict(){
		return statisticsDict;
	}

	private void InitStat(){
		statisticsDict ["numberOfTerminalPlaced"] = 0;
		statisticsDict ["numberOfTerminalBuffed"] = 0;
		statisticsDict ["numberOfTerminalDamaged"] = 0;
		statisticsDict ["numberOfZoneHealed"] = 0;
	}


	public Statistics (int numberOfTerminalPlaced, int numberOfTerminalBuffed, int numberOfTerminalDamaged, int numberOfZoneHealed)
	{
		statisticsDict ["numberOfTerminalPlaced"] = numberOfTerminalPlaced;
		statisticsDict ["numberOfTerminalBuffed"] = numberOfTerminalBuffed;
		statisticsDict ["numberOfTerminalDamaged"] = numberOfTerminalDamaged;
		statisticsDict ["numberOfZoneHealed"] = numberOfZoneHealed;
	}
}


