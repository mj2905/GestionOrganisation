﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ImproveButtonHandler : LocationListener {

	private Button improveButton;
	private Text improveButtonText;
	private bool attackMode = false;
	public enum TARGET {NO_TARGET, TERMINAL, ZONE};
	private TARGET targeting;
	private bool sameTeam;
	private int credits;

	private int terminalHealth;
	private int terminalLevel;
	private int zoneHealth;
	private int zoneLevel;

	void Start() {
		improveButton = GetComponent<Button> ();
		improveButtonText = improveButton.transform.Find ("Text").GetComponent<Text> ();
		targeting = TARGET.NO_TARGET;
		sameTeam = false;
		credits = 0;
	}

	void Update() {
		if (attackMode || targeting == TARGET.NO_TARGET) {
			improveButtonText.text = "✕";
			improveButton.interactable = false;
		} else if(targeting == TARGET.TERMINAL) {
			if (sameTeam) {

				if (terminalHealth <= QuantitiesConstants.TERMINAL_MIN_HEALTH) {
					improveButtonText.text = "Improve ✝";
					improveButton.interactable = false;
				} else if (terminalLevel == QuantitiesConstants.TERMINAL_MAX_HEALTH_VALUES.Length) {
					improveButtonText.text = "Improve ✓";
					improveButton.interactable = false;
				} else if (credits < QuantitiesConstants.TERMINAL_MAX_HEALTH_COST[terminalLevel + 1]) {
					improveButtonText.text = "Improve "+ QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [terminalLevel + 1] +"₡";
					improveButton.interactable = false;
				} else {
					improveButtonText.text = "Improve "+ QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [terminalLevel + 1] +"₡";
					improveButton.interactable = true;
				}
			} else {
				improveButtonText.text = "✕";
				improveButton.interactable = false;
			}
		} else if(targeting == TARGET.ZONE) {
			if (sameTeam) {
				if (zoneHealth <= QuantitiesConstants.ZONE_MIN_HEALTH) {
					improveButtonText.text = "Improve ✝";
					improveButton.interactable = false;
				} else if (zoneLevel < QuantitiesConstants.ZONE_MAX_HEALTH_VALUES.Length) {
					if(credits >= QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1]) {
						improveButtonText.text = "Improve "+ QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1] +"₡";
						improveButton.interactable = true;
					}
					else {
						improveButtonText.text = "Improve "+ QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1] +"₡";
						improveButton.interactable = false;
					}
				} else {
					improveButtonText.text = "Improve ✓";
					improveButton.interactable = false;
				}
			} else {
				improveButtonText.text = "✕";
				improveButton.interactable = false;
			}
		}
	}

	public void targetingZone(bool sameTeam, int zoneHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetZoneHealth (zoneHealth, level);
		setCredits (credits);
		this.sameTeam = sameTeam;
		targeting = TARGET.ZONE;
	}

	public void targetingTerminal(bool sameTeam, int terminalHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetTerminalHealth (terminalHealth, level);
		setCredits (credits);
		this.sameTeam = sameTeam;
		targeting = TARGET.TERMINAL;
	}

	public void targetingNothing() {
		Assert.IsTrue (!attackMode);
		targeting = TARGET.NO_TARGET;
	} 

	public void setCredits(int credits) {
		this.credits = credits;
	}

	public void setTargetTerminalHealth(int terminalHealth, int level) {
		this.terminalHealth = terminalHealth;
		this.terminalLevel = level;
	}

	public void setTargetZoneHealth(int zoneHealth, int level) {
		this.zoneHealth = zoneHealth;
		this.zoneLevel = level;
	}


	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		attackMode = false;
	}

	override public void FirstLocationSent() {
		attackMode = true;
	}
}