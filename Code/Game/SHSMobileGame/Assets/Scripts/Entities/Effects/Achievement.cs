using System;
using System.Collections.Generic;

public class Achievement{
	private int multiplier;
	private int ttl = QuantitiesConstants.TTL_ACHIEVEMENT;
	private string name;
		
	public Achievement (string name,int multiplier)
	{
		this.multiplier = multiplier;
		this.name = name;
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
		if (obj.GetType () != typeof(Achievement))
			return false;
		Achievement other = (Achievement)obj;
		return name == other.name;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return (name != null ? name.GetHashCode () : 0);
		}
	}

	public string GetName(){
		return name;
	}

	public Dictionary<string, object> ToMap() {
		Dictionary<string, object> fields = new Dictionary<string, object>();
		fields.Add ("multiplier",multiplier);
		fields.Add ("ttl",ttl);
		return fields;
	}
}


