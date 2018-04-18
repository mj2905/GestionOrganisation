using System.Collections.Generic;
using System;

public class SkinsInfo{

	public List<int> boughtPlayer = new List<int>();
	
	public SkinsInfo (System.Object skins){
		if (skins != null) {
			IDictionary<string,System.Object> objectDict = (IDictionary<string,System.Object>)skins;
			string boughtPlayersString = objectDict ["boughtPlayers"].ToString();
			boughtPlayer = ConstructListFromString (boughtPlayersString);
		}
	}

	public List<int> ConstructListFromString(string s){
		List<int> res = new List<int>();
		foreach (var c in s) {
			res.Add (Int32.Parse (c.ToString ()));
		}
		return res;
	}

	public string ConstructStringFromList(List<int> l){
		string res = "";
		foreach (var elem in l) {
			res += elem.ToString ();
		}
		return res;
	}

	public void addPlayerSkin(int number){
		boughtPlayer.Add (number);
	}
		
	public Dictionary<string, object> ToMap() {
		Dictionary<string, object> fields = new Dictionary<string, object>();
		fields.Add ("boughtPlayers",ConstructStringFromList(boughtPlayer));
		return fields;
	} 
}
