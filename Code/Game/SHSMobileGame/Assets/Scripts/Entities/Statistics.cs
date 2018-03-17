using System;
using System.Collections.Generic;

public class Statistics{

	private int numberOfTerminalPlaced = 0;
	private int numberOfTerminalBuffed = 0;
	private int numberOfTerminalDamaged = 0;
	private int numberOfZoneHealed = 0;

	public Statistics (System.Object statistics){
		if (statistics != null) {
			IDictionary<string,System.Object> statisticsDict = (IDictionary<string,System.Object>)statistics;
			foreach (KeyValuePair<string, System.Object> entry in statisticsDict) {
				switch (entry.Key) {
				case "numberOfTerminalPlaced":
					numberOfTerminalPlaced = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfTerminalBuffed":
					numberOfTerminalBuffed = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfTerminalDamaged":
					numberOfTerminalDamaged = Int32.Parse (entry.Value.ToString ());
					break;
				case "numberOfZoneHealed":
					numberOfZoneHealed = Int32.Parse (entry.Value.ToString ());
					break;
				}
			}
		}
	}

	public Statistics (int numberOfTerminalPlaced, int numberOfTerminalBuffed, int numberOfTerminalDamaged, int numberOfZoneHealed)
	{
		this.numberOfTerminalPlaced = numberOfTerminalPlaced;
		this.numberOfTerminalBuffed = numberOfTerminalBuffed;
		this.numberOfTerminalDamaged = numberOfTerminalDamaged;
		this.numberOfZoneHealed = numberOfZoneHealed;
	}
}


