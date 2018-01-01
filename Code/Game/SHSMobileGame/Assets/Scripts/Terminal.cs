using System;
using System.Collections.Generic;
using UnityEngine;


public class Terminal : MonoBehaviour
	{
	public GameObject target;
	private string terminalId;
	public GameObject effectPrefab;
	public string zoneId;
	public int strength;
	public int hp;
	public int team;

	void Start(){
	}

	public void Init(){
		if (target != null) {

			Vector3 targetPos = target.gameObject.transform.position;
			Vector3 targetLookAtPos = new Vector3 (targetPos.x, gameObject.transform.position.y, targetPos.z); 
			gameObject.transform.LookAt(targetLookAtPos);

			Instantiate (
				effectPrefab,
				gameObject.transform.position + new Vector3(0,2,0),
				Quaternion.LookRotation(targetPos - gameObject.transform.position),
				gameObject.transform);
			 
			print ("Instantiated prefab");
		}
	}

	void Update(){
	}

	public Terminal(){
	}

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


