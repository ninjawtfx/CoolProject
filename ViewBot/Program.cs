using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ViewBot
{
	class Program
	{
		static void Main(string[] args)
		{
			

			while (true)
			{
				OpenUrl();	
			}
			
			
			Console.Read();
		}

		static async void OpenUrl()
		{
			Process cmd = new Process();
			cmd.StartInfo.FileName = "livestreamer.exe";
			cmd.StartInfo.Arguments = "--http-header Client-ID=ewvlchtxgqq88ru9gmfp1gmyt6h2b93 twitch.tv/risahashi -j";

			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();

			string s = cmd.StandardOutput.ReadToEnd();

			RootObject ob = JsonConvert.DeserializeObject<RootObject>(s);

			WebRequest request = WebRequest.Create(ob.Streams.Audio.Url);
			request.Timeout = 100;

			try
			{
				var res = await request.GetResponseAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.WriteLine(request.Headers);
		}
	}
}
