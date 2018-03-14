using System;


public class EffectsConstants
{
	public static Medal medal101 = new Medal("101",5,10);	
	public static Medal medal202 = new Medal("202",1,10);	
	public static Medal medal404 = new Medal("404",2,15);	
	public static Medal medal505 = new Medal("505",4,5);	


	public static Medal GetMedalByName(string name){
		switch (name) {
		case "101":
			return medal101;
		case "202":
			return medal202;
		case "404":
			return medal404;
		case "505":
			return medal505;
		}

		return null;
	}
}

