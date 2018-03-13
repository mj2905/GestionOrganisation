using System;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour{

	public Image imageTime;
	public Text textMult;

	private int ttl;
	private int multiplier;
	private string name;
	private float height;
	private RectTransform rectTransform;
	private Vector2 offsetMax;

	public Medal (string name, int multiplier,int ttl)
	{
		this.name = name;
		this.ttl = ttl;
		this.multiplier = multiplier;
	}	

	public string GetName(){
		return name;
	}

	public int GetTtl(){
		return ttl;
	}

	public int GetMultiplier(){
		return multiplier;
	}

	public void Copy(Medal medal){
		ttl = medal.GetTtl ();
		multiplier = medal.GetMultiplier ();
		name = medal.GetName ();
	}

	public void Start(){
		rectTransform = imageTime.GetComponent<RectTransform> ();
		height = rectTransform.rect.height;
		offsetMax = rectTransform.offsetMax;

		textMult.text = "x " + multiplier;
	}
		
	public void Update(){
		rectTransform.offsetMax = new Vector2(0,(-1+(float)(this.GetTtl ()) / (float)(EffectsConstants.GetMedalByName (this.GetName ()).GetTtl ())) * height);
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Medal))
			return false;
		Medal other = (Medal)obj;
		return name == other.name;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return (name != null ? name.GetHashCode () : 0);
		}
	}
	
}

