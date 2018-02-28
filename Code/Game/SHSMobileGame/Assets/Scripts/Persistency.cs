using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class Persistency {

	private static string fileNameUser = Path.Combine( Application.persistentDataPath, "1.shs" );
	private static string fileNamePass = Path.Combine( Application.persistentDataPath, "2.shs" );
	private static string fileNameAttack = Path.Combine( Application.persistentDataPath, "3.shs" );

	public static void Write(string username, string password) {
		Session session = new Session (username, password);
		session.Write(fileNameUser, fileNamePass);
	}

	public static bool Exists() {
		return File.Exists (fileNameUser) && File.Exists (fileNamePass);
	}

	public static Session Read() {
		return Session.Read (fileNameUser, fileNamePass);
	}

	public static void Erase() {
		File.Delete (fileNameUser);
		File.Delete (fileNamePass);
		File.Delete (fileNameAttack);
	}



	public class Session {
		public string username { get; }
		public string password { get; }

		private static byte[] xor(byte[] input) {
			for (int i=0; i < input.Length; ++i) {
				input [i] ^= 0x55;
			}

			return input;
		}

		public Session(string username, string password) {
			this.username = username;
			this.password = password;
		}

		public void Write(string fileUser, string filePass) {
			byte[] user_array = xor(Encoding.UTF8.GetBytes (username));
			byte[] password_array = xor(Encoding.UTF8.GetBytes (password));

			File.WriteAllBytes (fileUser, user_array);
			File.WriteAllBytes (filePass, password_array);
		}

		public static Session Read(string fileUser, string filePass) {

			string username = Encoding.UTF8.GetString (xor(File.ReadAllBytes (fileUser)));
			string password = Encoding.UTF8.GetString (xor(File.ReadAllBytes (filePass)));

			return new Session(username, password);
		}

	}
}