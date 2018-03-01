using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour {

	public GameManager gameManager;

	private Zone targetedZone;
	private Terminal targetedTerminal;

	private Text popupZoneName;
	private Text popupZoneHP;
	private Text popupZoneLevel;
	private Text popupZoneTeam;
	private Text popupTerminalHP;
	private Text popupTerminalLevel;
	private Text popupTerminalTeam;
	private Text popupTerminalActionButtonText;

	public GameObject zonePopup;
	public GameObject terminalPopup;

	// Use this for initialization
	void Start () {
		targetedZone = null;
		targetedTerminal = null;

		popupZoneName = zonePopup.transform.Find ("ZoneLabel").GetComponent<Text> ();
		popupZoneHP = zonePopup.transform.Find ("HPLabel").GetComponent<Text> ();
		popupZoneLevel = zonePopup.transform.Find ("LevelLabel").GetComponent<Text> ();
		popupZoneTeam = zonePopup.transform.Find ("TeamLabel").GetComponent<Text> ();
		popupTerminalHP = terminalPopup.transform.Find ("HPLabel").GetComponent<Text> ();
		popupTerminalLevel = terminalPopup.transform.Find ("LevelLabel").GetComponent<Text> ();
		popupTerminalTeam = terminalPopup.transform.Find ("TeamLabel").GetComponent<Text> ();

		popupTerminalActionButtonText = terminalPopup.transform.Find ("ActionButton").transform.Find ("ActionButtonText").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (targetedZone != null) {
			popupZoneName.text = "Zone: " + targetedZone.name;
			popupZoneHP.text = "HP: " + targetedZone.health;
			popupZoneLevel.text = "Level: " + targetedZone.level;
			popupZoneTeam.text = "Team: " + targetedZone.team;
		}

		if (targetedTerminal != null) {
			popupTerminalHP.text = "HP: " + targetedTerminal.hp;
			popupTerminalLevel.text = "Level: " + targetedTerminal.level;
			popupTerminalTeam.text = "Team: " + targetedTerminal.team;
		}
	}

	public void updateTargetedZone(Zone zone){
		this.targetedZone = zone;

		if (targetedZone == null) {
			zonePopup.SetActive (false);
		} else {
			terminalPopup.SetActive (false);
			zonePopup.SetActive (true);
		}
	}

	public void updateTargetedTerminal(Terminal terminal){
		this.targetedTerminal = terminal;

		if (targetedTerminal == null) {
			terminalPopup.SetActive (false);
		} else {
			zonePopup.SetActive (false);
			terminalPopup.SetActive (true);

			if (gameManager.IsAttackMode ()) {
				popupTerminalActionButtonText.text = "Buff";
			} else {
				popupTerminalActionButtonText.text = "Smash";
			}
		}
	}

	public void healZone(){
		if (targetedZone != null) {
			print ("Healing zone " + targetedZone.name);
			FirebaseManager.HealZone (targetedZone.zoneId, QuantitiesConstants.ZONE_HEAL_AMOUNT);
		}
	}

	public void buffTerminal(){
		if(targetedTerminal != null){
			print ("Buffing terminal ");
			FirebaseManager.BuffTerminal (targetedTerminal.GetTerminalId (), QuantitiesConstants.TERMINAL_BUFF_AMOUNT);
		}
	}

	public void attackTerminal(){
		if(targetedTerminal != null){
			print ("Attacking terminal ");
			FirebaseManager.HurtTerminal (targetedTerminal.GetTerminalId (), QuantitiesConstants.TERMINAL_SMASH_AMOUNT);
		}
	}
}
