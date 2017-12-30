using System;
using System.Collections.Generic;

public class Zone
{
	private int zoneId;
	public int health;
	public int team;

	public Zone (int zoneId,int health, int team)
	{
		this.zoneId = zoneId;
		this.health = health;
		this.team = team;
	}

	public Zone (int zoneId, IDictionary<string,System.Object> entry)
	{
		this.zoneId = zoneId;
		this.health = Int32.Parse(entry["health"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
	}
}

