using System;

public class Medal{
	private int ttl;
	private int multiplier;

	public Medal (int multiplier,int ttl)
	{
		this.ttl = ttl;
		this.multiplier = multiplier;
	}	

	public int GetTtl(){
		return ttl;
	}

	public int GetMultiplier(){
		return multiplier;
	}
}

