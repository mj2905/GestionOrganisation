using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using System.Linq;


public class Game
{

	public List<Terminal> terminals = new List<Terminal> ();
	public List<Zone> zones = new List<Zone> ();
	public List<Team> teams = new List<Team> ();
	public List<User> bestUsers = new List<User> ();

	public Game (){
	}
		
	public Game (System.Object terminalsObject, System.Object zonesObject, System.Object teamsObject,System.Object bestPlayersObject)
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
		if (bestPlayersObject != null) {
			IDictionary<string,System.Object> bestPlayersList = (IDictionary<string,System.Object>)bestPlayersObject;
			foreach (KeyValuePair<string, System.Object> entry in bestPlayersList) {
				User user = new User (entry.Key, (IDictionary<string,System.Object>)entry.Value);
				bestUsers.Add (user);
			}
			bestUsers.Sort((a,b) => b.CompareTo(a));
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

	public int GetToken(int teamId) {
		int token = 0;
		foreach (Team team in teams) {
			if (team.getTeamId () == teamId) {
				token = team.token;
			}
		}
		return token;
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

	public IList<Zone> GetZones(){
		return this.zones.AsReadOnly();
	}

	public List<Zone> GetEnemyAttackingAllyZone(List<Terminal> terminals){
		HashSet<Zone> res = new HashSet<Zone>();
		foreach (Terminal terminal in terminals) {
			if (terminal.team != FirebaseManager.userTeam) {
				foreach (Zone zone in zones) {
					if (zone.zoneId == terminal.zoneId && zone.team == FirebaseManager.userTeam) {
						res.Add(zone);
					}
				}
			}
		}
		return res.ToList();
	}

	public List<Zone> GetAllyAttackingEnemyZone(List<Terminal> terminals){
		HashSet<Zone> res = new HashSet<Zone>();
		HashSet<string> zoneIds = new HashSet<string> ();

		foreach (Terminal terminal in terminals) {
			if (terminal.team == FirebaseManager.userTeam) {
				zoneIds.Add (terminal.zoneId);
			}
		}
		foreach (String id in zoneIds) {
			foreach (Zone zone in zones) {
				if (zone.zoneId == id) {
					res.Add(zone);
				}
			}
		}

		return res.ToList();
	}

	public List<Zone> GetNewAllyTerminals(Game pastGame){
		return GetAllyAttackingEnemyZone (GetNewTerminals(pastGame));
	}

	public List<Zone> GetDeletedAllyTerminals(Game pastGame){
		return GetAllyAttackingEnemyZone (GetDeletedTerminals(pastGame));
	}

	public List<Zone> GetNewEnemyTerminals(Game pastGame){
		return GetEnemyAttackingAllyZone (GetNewTerminals(pastGame));
	}

	public List<Zone> GetDeletedEnemyTerminals(Game pastGame){
		return GetEnemyAttackingAllyZone (GetDeletedTerminals(pastGame));
	}

	public List<int> GetScores(){
		List<int> res = new List<int> ();
		for (int i = 0; i < teams.Count; i++) {
			res.Add (teams [i].score);
		}
		return res;
	}
}

