using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InteractionManager : LocationListener {

	public GameManager gameManager;

	private Zone targetedZone;
	private Terminal targetedTerminal;
	private int credits;
	private int level;

	private Text popupZoneName;
	private Text popupZoneHP;
	private Text popupZoneLevel;
	private Text popupZoneTeam;

	private Text popupZoneDamagePercentBlue;
	private Text popupZoneDamagePercentRed;
	private Text popupZoneDamagePercentGreen;
	private Text popupZoneDamagePercentYellow;

	private Text popupTerminalHP;
	private Text popupTerminalLevel;
	private Text popupTerminalTeam;
	private Text popupTerminalStrength;

	public ActionButtonHandler actionButton;
	public ImproveButtonHandler improveButton;

	public GameObject zonePopup;
	public GameObject terminalPopup;

	public PopupScript messagePopup;

	private bool isAttackMode = false;
	private Terminal actualTerminal;

	// Use this for initialization
	void Start () {
		targetedZone = null;
		targetedTerminal = null;

		popupZoneName = zonePopup.transform.Find ("ZoneLabel").GetComponent<Text> ();
		popupZoneHP = zonePopup.transform.Find ("HPLabel").GetComponent<Text> ();
		popupZoneLevel = zonePopup.transform.Find ("LevelLabel").GetComponent<Text> ();
		popupZoneTeam = zonePopup.transform.Find ("TeamLabel").GetComponent<Text> ();

		popupZoneDamagePercentBlue = zonePopup.transform.Find ("DamagePercentBlue").GetComponent<Text> ();
		popupZoneDamagePercentRed = zonePopup.transform.Find ("DamagePercentRed").GetComponent<Text> ();
		popupZoneDamagePercentGreen = zonePopup.transform.Find ("DamagePercentGreen").GetComponent<Text> ();
		popupZoneDamagePercentYellow = zonePopup.transform.Find ("DamagePercentYellow").GetComponent<Text> ();

		popupTerminalHP = terminalPopup.transform.Find ("HPLabel").GetComponent<Text> ();
		popupTerminalLevel = terminalPopup.transform.Find ("LevelLabel").GetComponent<Text> ();
		popupTerminalStrength = terminalPopup.transform.Find ("StrengthLabel").GetComponent<Text> ();
		popupTerminalTeam = terminalPopup.transform.Find ("TeamLabel").GetComponent<Text> ();

		//terminalPopup.transform.localScale = new Vector3 (0, 0, 0);
	}

	private void UpdateDamagePercent(Text popupZoneDamagePercent,Damages damages,int damageTeam,int currentTeam){
		if (damageTeam == currentTeam) {
			popupZoneDamagePercent.text = "/";
		} else {

			if (damages.isDamaged ()) {
				popupZoneDamagePercent.text = (damages.GetDamage (damageTeam) / (float)(damages.getTotalDamages ()) * 100f).ToString ("0.0") + "%"; 
			} else {
				popupZoneDamagePercent.text = "0.0%"; 
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (targetedZone != null) {

			actionButton.setTargetZoneHealth (targetedZone.health, targetedZone.level);
			improveButton.setTargetZoneHealth(targetedZone.health, targetedZone.level);
			popupZoneName.text = targetedZone.name;
			popupZoneHP.text = "HP: " + targetedZone.health;
			popupZoneLevel.text = "Level: " + targetedZone.level;
			popupZoneTeam.text = ColorConstants.getColorAsString(targetedZone.team);
			popupZoneTeam.color = ColorConstants.getTextColor (targetedZone.team);

			UpdateDamagePercent (popupZoneDamagePercentGreen, targetedZone.damages, 1, targetedZone.team);
			UpdateDamagePercent (popupZoneDamagePercentRed, targetedZone.damages, 2, targetedZone.team);
			UpdateDamagePercent (popupZoneDamagePercentYellow, targetedZone.damages,3, targetedZone.team);
			UpdateDamagePercent (popupZoneDamagePercentBlue, targetedZone.damages, 4, targetedZone.team);

		}

		if (targetedTerminal != null) {
			actionButton.setTargetTerminalHealth (targetedTerminal.hp, targetedTerminal.level, targetedTerminal.strength);
			improveButton.setTargetTerminalHealth (targetedTerminal.hp, targetedTerminal.level);
			popupTerminalHP.text = "HP: " + targetedTerminal.hp;
			popupTerminalLevel.text = "Level: " + targetedTerminal.level;
			popupTerminalStrength.text = "Strength: " + targetedTerminal.strength;
			popupTerminalTeam.text = ColorConstants.getColorAsString(targetedTerminal.team);
			popupTerminalTeam.color = ColorConstants.getTextColor (targetedTerminal.team);
		}
	}

	public void updateUserInfo(string credits, string level) {
		int creditAsInt = Int32.Parse (credits);
		int levelAsInt = Int32.Parse (level);
		this.credits = creditAsInt;
		this.level = levelAsInt;
		actionButton.setCredits (creditAsInt);
		improveButton.setCredits (creditAsInt);
	}

	public void updateZoneInfo(Zone zone) {
		if (this.targetedZone != null && this.targetedZone.zoneId == zone.zoneId && this.targetedZone.team != zone.team) {
			updateTargetedZone (null);
		}
	}

	public void updateTargetedZone(Zone zone){
		this.targetedZone = zone;

		if (targetedZone == null) {
			zonePopup.SetActive (false);

			actionButton.targetingNothing ();
			improveButton.targetingNothing ();

		} else {
			if (targetedTerminal != null) {
				targetedTerminal.callbackWhenDestroyed = () => {};
				targetedTerminal = null;
			}

			terminalPopup.SetActive (false);//.transform.localScale = new Vector3 (0, 0, 0);
			zonePopup.SetActive (true);

			actionButton.targetingZone (FirebaseManager.userTeam == zone.team, targetedZone.health, targetedZone.level, credits);
			improveButton.targetingZone (FirebaseManager.userTeam == zone.team, targetedZone.health, targetedZone.level, credits);

		}
	}

	public void updateTargetedTerminal(Terminal terminal){
		Terminal oldTerminal = this.targetedTerminal;
		this.targetedTerminal = terminal;

		if (targetedTerminal == null) {

			if (oldTerminal != null) {
				oldTerminal.callbackWhenDestroyed = () => {};
				oldTerminal = null;
			}

			terminalPopup.SetActive (false);//.transform.localScale = new Vector3 (0, 0, 0);

			actionButton.targetingNothing ();
			improveButton.targetingNothing ();

		} else {
			terminal.callbackWhenDestroyed = () => updateTargetedTerminal (null);
			targetedZone = null;

			zonePopup.SetActive (false);
			terminalPopup.SetActive (true);//.transform.localScale = new Vector3 (1, 1, 1);

			actionButton.targetingTerminal (FirebaseManager.userTeam == targetedTerminal.team, targetedTerminal.hp, targetedTerminal.level, credits, targetedTerminal.strength);
			improveButton.targetingTerminal (FirebaseManager.userTeam == targetedTerminal.team, targetedTerminal.hp, targetedTerminal.level, credits);

		}
	}

	public bool isTerminalSelected(){
		return targetedTerminal != null;
	}

	public void healZone(){
		if (targetedZone != null) {
			print ("Healing zone " + targetedZone.name);
			actionButton.SetHealTimer ();
			FirebaseManager.HealZone (targetedZone.zoneId, QuantitiesConstants.ZONE_HEAL_AMOUNT, level, messagePopup);
		}
	}

	public void improveZone(){
		if (targetedZone != null) {
			print ("Improving zone " + targetedZone.name);
			improveButton.SetZoneTimer ();
			FirebaseManager.ImproveZone (targetedZone.zoneId, targetedZone.level, messagePopup);
		}
	}

	public void dispatchAction(){

		if (targetedZone != null) {
			healZone ();
		} else if (targetedTerminal != null) {
			if (FirebaseManager.userTeam == targetedTerminal.team) {
				buffTerminal ();
			} else {
				smashTerminal ();
			}
		} else {
			Debug.Log ("Neither a zone nor a terminal was selected upon call of action button");
		}
	}

	public void dispatchActionImprove(){

		if (targetedZone != null) {
			improveZone ();
		} else if (targetedTerminal != null) {
			if (FirebaseManager.userTeam == targetedTerminal.team) {
				improveTerminal ();
			}
		} else {
			Debug.Log ("Neither a zone nor a terminal was selected upon call of improve button");
		}
	}

	private void buffTerminal(){
		if(targetedTerminal != null){
			print ("Buffing terminal ");
			actionButton.SetBuffTimer ();
			FirebaseManager.BuffTerminal (targetedTerminal.GetTerminalId (), targetedTerminal.strength, messagePopup);
		}
	}

	private void smashTerminal(){
		if(targetedTerminal != null){
			print ("Attacking terminal with ID " + targetedTerminal.GetTerminalId());
			actionButton.SetSmashTimer ();
			FirebaseManager.HurtTerminal (targetedTerminal.GetTerminalId (), QuantitiesConstants.TERMINAL_SMASH_AMOUNT, level, messagePopup);
		}
	}
		
	private void improveTerminal(){
		if(targetedTerminal != null){
			print ("Improving terminal ");
			improveButton.SetTerminalTimer ();
			FirebaseManager.ImproveTerminal (targetedTerminal.GetTerminalId (), targetedTerminal.level, messagePopup);
		}
	}

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
	}
}
