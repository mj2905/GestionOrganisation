using System;
using System.Collections.Generic;


public class Team
{
	private int teamId;
	public int score;
	public int token;

	public Team (int teamId, int score, int token)
	{
		this.teamId = teamId;
		this.score = score;
		this.token = token;
	}

	public Team (int teamId, IDictionary<string,System.Object> entry)
	{
		this.teamId = teamId;
		this.score = Int32.Parse(entry["score"].ToString());
		this.token = Int32.Parse(entry["token"].ToString());
	}
}


