using System;
using System.Collections.Generic;

public class Damages{
	public int[] damages = new int[] {0, 0, 0, 0};

	public Damages(){}

	public Damages (IDictionary<string,System.Object> entry){
		if(entry.ContainsKey("1")){
			damages[0] =  Int32.Parse(entry["1"].ToString());
		}
		if(entry.ContainsKey("2")){
			damages[1] =  Int32.Parse(entry["2"].ToString());
		}
		if(entry.ContainsKey("3")){
			damages[2] =  Int32.Parse(entry["3"].ToString());
		}
		if(entry.ContainsKey("4")){
			damages[3] =  Int32.Parse(entry["4"].ToString());
		}
	}

	public bool isDamaged(){
		for (int i = 0; i < damages.Length; i++) {
			if (damages [i] > 0) {
				return true;
			}
		}
		return false;
	}

	public int getTeamHighestDamage(){
		int currentTeam = -1,currentMax = -1;

		for (int i = 0; i < damages.Length; i++) {
			if (currentMax < damages [i]) {
				currentTeam = i + 1;
				currentMax = damages [i];
			}
		}
		return currentTeam;
	}
}

