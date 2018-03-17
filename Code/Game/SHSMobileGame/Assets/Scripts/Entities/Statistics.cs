using System;

public class Statistics{

	private int numberOfTerminalPlaced;
	private int numberOfTerminalBuffed;
	private int numberOfTerminalDamaged;
	private int numberOfZoneHealed;

	public Statistics (){

	}

	public Statistics (int numberOfTerminalPlaced, int numberOfTerminalBuffed, int numberOfTerminalDamaged, int numberOfZoneHealed)
	{
		this.numberOfTerminalPlaced = numberOfTerminalPlaced;
		this.numberOfTerminalBuffed = numberOfTerminalBuffed;
		this.numberOfTerminalDamaged = numberOfTerminalDamaged;
		this.numberOfZoneHealed = numberOfZoneHealed;
	}
}


