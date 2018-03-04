using System;

public class User
	{
	public const int MAX_CREDITS = 10000;
	public const int MIN_CREDITS = 0;

	public int credits;
	public int level;
	public int xp;
	public int team;
		

	public User ()
	{
		this.credits = 0;
		this.level = 0;
		this.xp = 0;
		this.team = 0;
	}

	public User (int team)
	{
		this.credits = 0;
		this.level = 0;
		this.xp = 0;
		this.team = team;
	}
}


