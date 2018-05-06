using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zone : MonoBehaviour {

	public GameObject statsChart;
	public string zoneId;
	public string name;
	private Text levelText;
	public int health;
	public int level;
	public int team;
	public Damages damages;

	private Image flag;
	private Image donutBackground;
	private Image dmgENAC;
	private Image dmgSTI;
	private Image dmgFSB;
	private Image dmgICSV;

	private float alphaVal;
	private Color alphaReset;
	private Color alphaComp;

	private const bool ZONE_DEBUG = false;

	public float dmgTmp;
	public float dmg1;
	public float dmg2;
	public float dmg3;
	public float dmg4;

	private Color colorSwitching;

	private const float SPEED_OF_TRANSITION = 2f;
	private const float DEGREES = 360;
	private float timer = 0;

	enum state {Idle,LerpingToNormalColor,LerpingToDamagedColor,ColoredNormalColor,ColoredDamagedColor}
	private state currentState;

	void Start() {
		if(damages == null) {
			damages = new Damages();
		}

		alphaVal = 1.0f;
		alphaReset = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		alphaComp = new Color (0, 0, 0, 1);

		levelText = statsChart.transform.Find ("Level").GetComponent<Text> ();

		flag = statsChart.transform.Find ("Flag").GetComponent<Image> ();
		Transform fillRoot = statsChart.transform.Find ("Fills");
		donutBackground = fillRoot.GetComponent<Image>();
		dmgENAC = fillRoot.transform.Find ("FillENAC").GetComponent<Image> ();
		dmgSTI = fillRoot.transform.Find ("FillSTI").GetComponent<Image> ();
		dmgFSB = fillRoot.transform.Find ("FillFSB").GetComponent<Image> ();
		dmgICSV = fillRoot.transform.Find ("FillICSV").GetComponent<Image> ();

		dmgENAC.color = ColorConstants.getTextColor ((int)ColorConstants.TEAMS.ENAC);
		dmgSTI.color = ColorConstants.getTextColor ((int)ColorConstants.TEAMS.STI);
		dmgFSB.color = ColorConstants.getTextColor ((int)ColorConstants.TEAMS.FSB);
		dmgICSV.color = ColorConstants.getTextColor ((int)ColorConstants.TEAMS.ICSV);
	}

	void Update(){

		updateDonutChart ();

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

	private void updateDonutChart(){
		// Total damages in donut chart should match lost hp from zone
		// current hp / max hp gives the amount all the damage should sum to in [0,1] interval
		// compute proportion of total damaged for each team, multiply by the above value

		levelText.text = level.ToString();

		float hpProp = 1.0f - (float)(health) / (float) QuantitiesConstants.ZONE_MAX_HEALTH_VALUES[level];

		flag.color = ColorConstants.getTextColor ((int)team) * alphaReset + alphaComp;

		float totalDamages = 0;

		if (ZONE_DEBUG) {
			totalDamages = dmgTmp	;	
		} else {
			totalDamages = (float)damages.getTotalDamages ();
		}

		if (totalDamages <= 0.01) {
			dmgENAC.fillAmount = 0;
			dmgSTI.fillAmount = 0;
			dmgFSB.fillAmount = 0;
			dmgICSV.fillAmount = 0;
			return;
		}

		if (ZONE_DEBUG) {
			dmgENAC.fillAmount = (dmg1 / totalDamages) * hpProp;
			dmgSTI.fillAmount = (dmg2 / totalDamages)  * hpProp;
			dmgFSB.fillAmount = (dmg3 / totalDamages)  * hpProp;
			dmgICSV.fillAmount = (dmg4 / totalDamages)  * hpProp;
		}else{
			dmgENAC.fillAmount = (damages.GetDamage ((int)ColorConstants.TEAMS.ENAC) / totalDamages) * hpProp;
			dmgSTI.fillAmount = (damages.GetDamage ((int)ColorConstants.TEAMS.STI) / totalDamages) * hpProp;
			dmgFSB.fillAmount = (damages.GetDamage ((int)ColorConstants.TEAMS.FSB) / totalDamages) * hpProp;
			dmgICSV.fillAmount = (damages.GetDamage ((int)ColorConstants.TEAMS.ICSV) / totalDamages) * hpProp;
		}


		dmgSTI.transform.localEulerAngles = new Vector3(0, 0, -dmgENAC.fillAmount * DEGREES);
		dmgFSB.transform.localEulerAngles = new Vector3(0, 0, -(dmgENAC.fillAmount + dmgSTI.fillAmount) * DEGREES);
		dmgICSV.transform.localEulerAngles = new Vector3(0, 0, -(dmgENAC.fillAmount + dmgSTI.fillAmount + dmgFSB.fillAmount) * DEGREES);
	}

	public Zone (string zoneId, int level, int health, int team,Damages damages)
	{
		this.zoneId = zoneId;
		this.health = health;
		this.level = level;
		this.team = team;
		this.damages = damages;
	}

	public Zone (string zoneId, IDictionary<string,System.Object> entry)
	{
		this.zoneId = zoneId;
		this.health = Int32.Parse(entry["health"].ToString());
		this.level = Int32.Parse(entry["level"].ToString());
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

	public void hideChart(bool hide){

		alphaVal = hide ? 0.15f : 1.0f;
		alphaComp = new Color (0, 0, 0, alphaVal);

		donutBackground.color = (donutBackground.color * alphaReset) + alphaComp;
		dmgENAC.color = (dmgENAC.color * alphaReset) + alphaComp;
		dmgSTI.color = (dmgSTI.color * alphaReset) + alphaComp;
		dmgFSB.color = (dmgFSB.color * alphaReset) + alphaComp;
		dmgICSV.color = (dmgICSV.color * alphaReset) + alphaComp;
	}
}

