using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.Core
{
	internal class HypixelCertificateHandling
	{
		public static string Deobfuscate(string str)
		{
			var bytes = Convert.FromBase64String(str);
			for (int i = 0; i < bytes.Length; i++) bytes[i] ^= 0x5a;
			return Encoding.UTF8.GetString(bytes);
		}

		public static string Sha256Encode(string rawData)
		{
			// Create a SHA256   
			using (SHA256 sha256Hash = SHA256.Create())
			{
				// ComputeHash - returns byte array  
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

				// Convert byte array to a string   
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
		public class JsonStruct
		{
			public JsonStruct(string hash, int time, bool copy, bool online)
			{
				Hash = hash;
				Time = time;
				Copy = copy;
				Online = online;
			}
			public string Hash { get; set; }
			public int Time { get; set; }
			public bool Copy { get; set; }
			public bool Online { get; set; }
		}

		/// <summary>
		/// Read how many api requests are left in last minute and await their expire date as to not get api-banned...
		/// </summary>
		/// <returns></returns>
		public static JsonStruct GetApiData()
		{
			try
			{
				var http = new HttpClient();
				var str = http.GetAsync("https://raw.githubusercontent.com/wisniax/YAHAC/master/YAHAC/Resources/Fonts/HypixelSpecialFont.ttf").Result.Content.ReadAsStringAsync();
				str.Wait();
				var des = JsonSerializer.Deserialize<List<JsonStruct>>(Deobfuscate(str.Result));
				if (des == null) return new JsonStruct("", 30, false, false);
				return des.FirstOrDefault((a) => a.Hash == Sha256Encode(MainViewModel.Settings.Default.BetaTests), new JsonStruct("", 30, false, false));
			}
			catch (Exception)
			{
				//new JsonStruct("", 30, false)
				return new JsonStruct("", 30, false, false);
			}
		}
	}
}
