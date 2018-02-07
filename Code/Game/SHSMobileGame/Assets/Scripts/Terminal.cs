using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
	{
	private GameObject target;
	private string terminalId;
	public string zoneId;
	public int strength;
	public int hp;
	public int team;
	public float x;
	public float z;

	public Image healthBar;
	public Image levelBar;
	public Image powerBar;

	void Start(){
	}

	public void Init(){
		if (target != null) {

			Vector3 targetPos = target.gameObject.transform.position;
			Vector3 targetLookAtPos = new Vector3 (targetPos.x, gameObject.transform.position.y, targetPos.z); 
			gameObject.transform.LookAt(targetLookAtPos);

			string color = "";
			switch(team){
			case 1:
				color = "Green";
				break;
			case 2:
				color = "Red";
				break;
			case 3:
				color = "Yellow";
				break;
			case 4:
				color = "Blue";
				break;
			default:
				color = "Blue";
				break;
			}

			Instantiate (
				Resources.Load("Effects/Prefabs/"+color+"LaserEffect"),
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

	public Terminal (string terminalId, string zoneId, int strength,int hp, int team,float x,float z)
	{
		this.terminalId = terminalId;
		this.zoneId = zoneId;
		this.strength = strength;
		this.hp = hp;
		this.team = team;
		this.x = x;
		this.z = z;
	}

	public Terminal (string terminalId, IDictionary<string,System.Object> entry)
	{
		this.terminalId = terminalId;
		this.zoneId = entry["zoneId"].ToString();
		this.strength = Int32.Parse(entry["strength"].ToString());
		this.hp = Int32.Parse(entry["hp"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
		this.x = float.Parse(entry["x"].ToString());
		this.z = float.Parse(entry["z"].ToString());
	}

	public string GetTerminalId(){
		return terminalId;
	}

	public void SetTarget(GameObject gameObject){
		this.target = gameObject;
	}
}


