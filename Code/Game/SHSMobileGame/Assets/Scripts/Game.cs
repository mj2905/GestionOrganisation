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

	public Game (){
	}
		
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
			IDictionary<string,System.Object> zonesList = (IDictionary<string,System.Object>)zonesObject;
			foreach (KeyValuePair<string, System.Object> entry in zonesList) {
				Zone zone = new Zone (entry.Key, (IDictionary<string,System.Object>)entry.Value);
				zones.Add (zone);
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

	public List<string> GetTerminalsName(){
		List<string> res = new List<string> ();
		for (int i = 0; i < this.terminals.Count; i++) {
			res.Add (this.terminals[i].GetTerminalId());
		}
		return res;
	}

	private List<Terminal> GetDifferenceTerminals(Game oldGame){
		List<Terminal> terminals = new List<Terminal> ();

		HashSet<string> oldTerminals = new HashSet<string> (oldGame.GetTerminalsName());
		HashSet<string> newTerminals = new HashSet<string> (this.GetTerminalsName());

		newTerminals.ExceptWith (oldTerminals);

		for (int i = 0; i < this.terminals.Count; i++) {
			Terminal currentTerminal = this.terminals [i];
			if (newTerminals.Contains (currentTerminal.GetTerminalId ())) {
				terminals.Add (this.terminals [i]);
			}
		}
		return terminals;
	}

	public List<Terminal> GetModifiedTerminals(Game oldGame){
		List<Terminal> terminals = new List<Terminal> ();
		
		HashSet<string> oldTerminals = new HashSet<string> (oldGame.GetTerminalsName());
		HashSet<string> newTerminals = new HashSet<string> (this.GetTerminalsName());
		
		newTerminals.IntersectWith (oldTerminals);
		for (int i = 0; i < this.terminals.Count; i++) {
			for (int j = 0; j < oldGame.terminals.Count; j++) {
				Terminal currentTerminal = this.terminals [i];
				Terminal currentOldTerminal = oldGame.terminals [j];
				if (newTerminals.Contains (currentTerminal.GetTerminalId ()) 
					&& currentTerminal.GetTerminalId() == currentOldTerminal.GetTerminalId()
					&& !currentTerminal.Equals(currentOldTerminal)) {
					terminals.Add (currentTerminal);
				}
			}
		}
		return terminals;
	}

	public List<Terminal> GetNewTerminals(Game oldGame){
		return this.GetDifferenceTerminals (oldGame);
	}

	public List<Terminal> GetDeletedTerminals(Game oldGame){
		return oldGame.GetDifferenceTerminals (this);
	}

}

