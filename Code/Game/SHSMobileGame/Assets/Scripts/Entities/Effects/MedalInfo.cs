using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MedalInfo{

	private int ttl;
	private int multiplier;
	private string name;

	public MedalInfo (string name, int multiplier,int ttl)
	{
		this.name = name;
		this.ttl = ttl;
		this.multiplier = multiplier;
	}

	public string GetName(){
		return name;
	}

	public string GetNormalisedName(){
		return Regex.Replace(name, @"[\d-]", string.Empty);
	}

	public int GetTtl(){
		return ttl;
	}

	public int GetMultiplier(){
		return multiplier;
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(MedalInfo))
			return false;
		MedalInfo other = (MedalInfo)obj;
		return name == other.name;
	}

	public override int GetHashCode ()
	{
		unchecked {
			return (name != null ? name.GetHashCode () : 0);
		}
	}
		
	public Dictionary<string, object> ToMap() {
		Dictionary<string, object> fields = new Dictionary<string, object>();
		fields.Add ("multiplier",multiplier);
		fields.Add ("ttl",ttl);
		return fields;
	}
}
