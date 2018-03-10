using System;

public class Achievement{
	private int multiplier;
	private string name;
		
	public Achievement (int multiplier, string name)
	{
		this.multiplier = multiplier;
		this.name = name;
	}
		
	public int GetMultiplier(){
		return multiplier;
	}
}


