using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : LocationListener {

	public GameManager gameManager;

	private Zone targetedZone;
	private Terminal targetedTerminal;

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
	private Text actionButtonText;

	public Button actionButton;
	public GameObject zonePopup;
	public GameObject terminalPopup;

	private bool isAttackMode = false;

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

		actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
	}

	private void UpdateDamagePercent(Text popupZoneDamagePercent,Damages damages,int damageTeam,int currentTeam){
		if (damageTeam == currentTeam) {
			popupZoneDamagePercent.text = "/";
		} else {
			if (damages.isDamaged ()) {
				Debug.Log (damages.GetDamage (damageTeam) / (float)(damages.getTotalDamages ()) * 100f);
				popupZoneDamagePercent.text = (damages.GetDamage (damageTeam) / (float)(damages.getTotalDamages ()) * 100f).ToString ("0.0") + "%"; 
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (targetedZone != null) {
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
			popupTerminalHP.text = "HP: " + targetedTerminal.hp;
			popupTerminalLevel.text = "Level: " + targetedTerminal.level;
			popupTerminalStrength.text = "Strength: " + targetedTerminal.strength;
			popupTerminalTeam.text = "Team: " + targetedTerminal.team;
		}
	}

	public void updateTargetedZone(Zone zone){
		this.targetedZone = zone;

		if (targetedZone == null) {
			zonePopup.SetActive (false);
			actionButtonText.text = "No action";
			actionButton.interactable = false;
		} else {
			targetedTerminal = null;
			terminalPopup.SetActive (false);
			zonePopup.SetActive (true);
			actionButtonText.text = "Heal";
			actionButton.interactable = true;
		}
	}

	public void updateTargetedTerminal(Terminal terminal){
		this.targetedTerminal = terminal;

		if (targetedTerminal == null) {
			terminalPopup.SetActive (false);
			actionButtonText.text = "No action";
			actionButton.interactable = false;
		} else {
			targetedZone = null;
			zonePopup.SetActive (false);
			terminalPopup.SetActive (true);
			actionButton.interactable = true;
			if (FirebaseManager.userTeam == targetedTerminal.team) {
				actionButtonText.text = "Buff";
			} else {
				actionButtonText.text = "Smash";
			}
		}
	}

	public void healZone(){
		if (targetedZone != null) {
			print ("Healing zone " + targetedZone.name);
			FirebaseManager.HealZone (targetedZone.zoneId, QuantitiesConstants.ZONE_HEAL_AMOUNT);
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

	private void buffTerminal(){
		if(targetedTerminal != null){
			print ("Buffing terminal ");
			FirebaseManager.BuffTerminal (targetedTerminal.GetTerminalId (), QuantitiesConstants.TERMINAL_BUFF_AMOUNT);
		}
	}

	private void smashTerminal(){
		if(targetedTerminal != null){
			print ("Attacking terminal ");
			FirebaseManager.HurtTerminal (targetedTerminal.GetTerminalId (), QuantitiesConstants.TERMINAL_SMASH_AMOUNT);
		}
	}

	override public void CoordinateUpdate(MapCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
	}
}
