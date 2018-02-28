using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour {

	public const float HP_MAX = 1000f;
	public const float LEVEL_MAX = 5f;

	public string zoneId;
	public string name;
	public int health;
	public int level;
	public int team;
	public Image healthBar;
	public Image levelBar;

	void Start() {
	}

	void Update(){
		healthBar.fillAmount = (float)(health) / HP_MAX;
		levelBar.fillAmount =  (float)(level) / LEVEL_MAX;
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

	public void updateStatus(int newHealth, int newLevel, int newTeam){
		this.health = newHealth;
		this.level = newLevel;
		this.team = newTeam;
	}
}

