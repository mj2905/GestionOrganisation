using System;
using System.Collections.Generic;


public class Terminal 
	{

	private string terminalId;
	public string zoneId;
	public int strength;
	public int hp;
	public int team;

	public Terminal (string terminalId, string zoneId, int strength,int hp, int team)
	{
		this.terminalId = terminalId;
		this.zoneId = zoneId;
		this.strength = strength;
		this.hp = hp;
		this.team = team;
	}

	public Terminal (string terminalId, IDictionary<string,System.Object> entry)
	{
		this.terminalId = terminalId;
		this.zoneId = entry["zoneId"].ToString();
		this.strength = Int32.Parse(entry["strength"].ToString());
		this.hp = Int32.Parse(entry["hp"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
	}

	public string GetTerminalId(){
		return terminalId;
	}
}


