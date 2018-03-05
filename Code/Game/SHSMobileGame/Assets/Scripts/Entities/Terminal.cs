using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
	{

	private const float HP_MAX = 100f;
	private const float LEVEL_MAX = 5f;
	private const float POWER_MAX = 100f;

	private GameObject target;
	private string terminalId;
	public string zoneId;
	public int level;
	public int strength;
	public int hp;
	public int team;
	public float x;
	public float z;
	public int[] damages;

	private Image healthBar;
	private Image levelBar;
	private Image powerBar;

	void Start(){
		//terminalId = "Terminal";
		healthBar = GameObject.Find (terminalId+"/TowerStatsCanvas/HealthBG/HealthBar").GetComponent<Image>();
		levelBar = GameObject.Find (terminalId+"/TowerStatsCanvas/LevelBG/LevelBar").GetComponent<Image>();
		powerBar = GameObject.Find (terminalId+"/TowerStatsCanvas/PowerBG/PowerBar").GetComponent<Image>();
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
		healthBar.fillAmount = (float)(hp) / HP_MAX;
		levelBar.fillAmount =  (float)(level) / LEVEL_MAX;
		powerBar.fillAmount = (float)(strength) / POWER_MAX;
	}

	public Terminal(){
	}

	public Terminal (string terminalId, string zoneId, int level, int strength, int hp, int team, float x, float z)
	{
		this.terminalId = terminalId;
		this.zoneId = zoneId;
		this.level = level;
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
		this.level = Int32.Parse(entry["level"].ToString());
		this.hp = Int32.Parse(entry["hp"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
		this.x = float.Parse(entry["x"].ToString());
		this.z = float.Parse(entry["z"].ToString());
	}

	public void Copy(Terminal other){
		this.name = other.GetTerminalId ();
		this.zoneId = other.zoneId;
		this.level = other.level;
		this.strength = other.strength;
		this.hp = other.hp;
		this.team = other.team;
		this.SetTerminalId (other.GetTerminalId ());
		this.x = other.x;
		this.z = other.z;
	}

	public string GetTerminalId(){
		return terminalId;
	}

	public void SetTerminalId(String terminalId){
		this.terminalId = terminalId;
	}

	public void SetTarget(GameObject gameObject){
		this.target = gameObject;
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Terminal))
			return false;
		Terminal other = (Terminal)obj;
		return terminalId == other.terminalId && zoneId == other.zoneId && level == other.level && strength == other.strength && hp == other.hp && team == other.team && x == other.x && z == other.z;
	}	

	public override int GetHashCode ()
	{
		unchecked {
			return (terminalId != null ? terminalId.GetHashCode () : 0) ^ (zoneId != null ? zoneId.GetHashCode () : 0) ^ level.GetHashCode () ^ strength.GetHashCode () ^ hp.GetHashCode () ^ team.GetHashCode () ^ x.GetHashCode () ^ z.GetHashCode ();
		}
	}

	public Dictionary<string, object> ToMap() {
		Dictionary<string, object> fields = new Dictionary<string, object>();
		fields.Add ("hp", hp);
		fields.Add ("level", level);
		fields.Add ("strength", strength);
		fields.Add ("team", team);
		fields.Add ("x", x);
		fields.Add ("z", z);
		fields.Add ("zoneId", zoneId);
		return fields;
	}
	
}


