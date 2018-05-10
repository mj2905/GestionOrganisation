using System;
using System.Collections.Generic;


public class User
	{
	public const int MAX_CREDITS = 10000;
	public const int MIN_CREDITS = 0;

	public int credits;
	public int level;
	public int xp;
	public int team;
	public string pseudo;

	private string userId; 
		
	public User ()
	{
		this.credits = 0;
		this.level = 0;
		this.xp = 0;
		this.pseudo = "";
		this.team = 0;
	}

	public User (string userId, IDictionary<string,System.Object> entry)
	{
		this.userId = userId;
		this.credits = Int32.Parse(entry["credits"].ToString());
		this.level = Int32.Parse(entry["level"].ToString());
		this.xp = Int32.Parse(entry["xp"].ToString());
		this.team = Int32.Parse(entry["team"].ToString());
		this.pseudo = entry["pseudo"].ToString();
	}

	public User (int team, string pseudo,string userId, int credits)
	{
		this.credits = credits;
		this.level = 0;
		this.xp = 0;
		this.pseudo = pseudo;
		this.team = team;
		this.userId = userId;
	}

	public string GetId(){
		return userId;
	}

	public int CompareTo(User other){
		if (this.xp == other.xp) {
			return this.credits - other.credits;
		} else {
			return this.xp - other.xp;
		}
	}
}


