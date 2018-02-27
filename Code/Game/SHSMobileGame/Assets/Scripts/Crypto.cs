using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using System.Text;

public static class Crypto {

	private static SHA256Managed sha = new SHA256Managed ();

	public static string hash(string username, string password, int iterator = 20) {
		byte[] user = System.Text.Encoding.UTF8.GetBytes (username);
		byte[] pass = System.Text.Encoding.UTF8.GetBytes (password);
		byte[] total = new byte[user.Length + password.Length];
		Buffer.BlockCopy (user, 0, total, 0, user.Length);
		Buffer.BlockCopy (pass, 0, total, user.Length, pass.Length);

		for (int i = 0; i < iterator; ++i) {
			total = sha.ComputeHash (total);
		}
		return Encoding.UTF8.GetString(total);
	}

}
