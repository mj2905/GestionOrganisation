using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour {

	private string zoneId;
	public int health;
	public int team;
	public Image healthBar;
	public Image levelBar;

	void Start() {
		
	}

	public Zone (string zoneId,int health, int team)
	{
		this.zoneId = zoneId;
		this.health = health;
		this.team = team;
	}

	public Zone (string zoneId, IDictionary<string,System.Object> entry)
	{
		this.zoneId = zoneId;
		this.health = Int32.Parse(entry["health"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
	}
}

