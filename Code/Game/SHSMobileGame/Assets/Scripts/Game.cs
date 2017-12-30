using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;


public class Game
{

	public List<Terminal> terminals = new List<Terminal> ();
	public List<Zone> zones = new List<Zone> ();
	public List<Team> teams = new List<Team> ();

	public Game (System.Object terminalsObject, System.Object zonesObject, System.Object teamsObject)
	{
		if (terminalsObject != null) {
			IDictionary<string,System.Object> terminalsList = (IDictionary<string,System.Object>)terminalsObject;
			foreach (KeyValuePair<string, System.Object> entry in terminalsList) {
				Terminal terminal = new Terminal (entry.Key, (IDictionary<string,System.Object>)entry.Value);
				terminals.Add (terminal);
			}
		}
		if (zonesObject != null) {
			List<System.Object> zonesList = (List<System.Object>)zonesObject;
			for (int i = 0; i < zonesList.Count; i++) {
				System.Object entry = zonesList [i]; 
				if (entry != null) {
					Zone zone = new Zone (i, (IDictionary<string,System.Object>)entry);
					zones.Add (zone);				
				}
			}
		}		
		if (teamsObject != null) {
			List<System.Object> teamsList = (List<System.Object>)teamsObject;
			for (int i = 0; i < teamsList.Count; i++) {
				System.Object entry = teamsList [i]; 
				if (entry != null) {
					Team team = new Team (i, (IDictionary<string,System.Object>)entry);
					teams.Add (team);				
				}
			}
		}		
	}

}

