using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour {

	public string zoneId;
	public string name;
	public int health;
	public int level;
	public int team;
	public Damages damages;
	public Image healthBar;
	public Image levelBar;

	private bool isSwitching = true;
	private bool isTimerIncreasing = true;

	private Color colorSwitching;

	private const float SPEED_OF_TRANSITION = 1f;
	private float timer = 0;

	enum state {Idle,LerpingToNormalColor,LerpingToDamagedColor,ColoredNormalColor,ColoredDamagedColor}
	private state currentState;

	void Start() {
	}

	void Update(){
		
		healthBar.fillAmount = (float)(health) / QuantitiesConstants.HP_MAX;
		levelBar.fillAmount =  (float)(level) / QuantitiesConstants.LEVEL_MAX;

		switch(currentState){
		case state.Idle:
			this.GetComponent<MeshRenderer> ().material.color = ColorConstants.getColor(this.team);
			if (damages != null && damages.isDamaged () && health < QuantitiesConstants.MINIMUM_HEALTH_SWITCH_COLOR) {
				currentState = state.LerpingToDamagedColor;
				colorSwitching = ColorConstants.getColor (damages.getTeamHighestDamage ());
				timer = 0;
			} 
			break;
		case state.LerpingToDamagedColor:
			timer += Time.deltaTime * SPEED_OF_TRANSITION;
			this.GetComponent<MeshRenderer> ().material.color = Color.Lerp (ColorConstants.getColor (this.team), colorSwitching, timer);
			if (timer > 1) {
				currentState = state.ColoredDamagedColor;
			}
			break;
		case state.ColoredDamagedColor:
			timer += Time.deltaTime * SPEED_OF_TRANSITION;
			if (timer > 1.5) {
				currentState = state.LerpingToNormalColor;
			}
			break;
		case state.LerpingToNormalColor:
			timer -= Time.deltaTime * SPEED_OF_TRANSITION;
			this.GetComponent<MeshRenderer> ().material.color = Color.Lerp (ColorConstants.getColor (this.team), colorSwitching, timer);
			if (timer < 0) {
				currentState = state.ColoredNormalColor;
			}
			break;
		case state.ColoredNormalColor:
			timer -= Time.deltaTime * SPEED_OF_TRANSITION;
			if (timer < -3) {
				currentState = state.Idle;
			}
			break;
		}
	}

	public Zone (string zoneId,int health, int team,Damages damages)
	{
		this.zoneId = zoneId;
		this.health = health;
		this.team = team;
		this.damages = damages;
	}

	public Zone (string zoneId, IDictionary<string,System.Object> entry)
	{
		this.zoneId = zoneId;
		this.health = Int32.Parse(entry["health"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
		if (entry.ContainsKey ("damages")) {
			if (entry ["damages"] as IDictionary<string,System.Object> != null) {
				IDictionary<string,System.Object> damagesDict = (IDictionary<string,System.Object>)entry ["damages"];
				this.damages = new Damages (damagesDict);
			} else {
				List<System.Object> damagesList = (List<System.Object>)entry ["damages"];
				this.damages = new Damages (damagesList);
			}
		} else {
			this.damages = new Damages ();
		}
	}

	public void Copy(Zone other){
		this.health = other.health;
		this.level = other.level;
		this.team = other.team;
		this.damages = other.damages;
	}
}

