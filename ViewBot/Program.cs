using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ViewBot
{
	class Program
	{
		static void Main(string[] args)
		{
			Parallel.For(0, 10, ConnectToStreamer);
			
			Console.Read();
		}

		public static void ConnectToStreamer(int i)
		{
			Process cmd = new Process();
			cmd.StartInfo.FileName = "livestreamer.exe";
			cmd.StartInfo.Arguments = "--http-header Client-ID=ewvlchtxgqq88ru9gmfp1gmyt6h2b93 twitch.tv/cmpunkxecw -j";

			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();

			string s = cmd.StandardOutput.ReadToEnd();

			RootObject ob = JsonConvert.DeserializeObject<RootObject>(s);

			WebRequest request = WebRequest.Create(ob.Streams.Audio.Url);
			request.Timeout = 100;

			while (true)
			{
				Task<WebResponse> res = res = request.GetResponseAsync();
				try
				{
					 
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				Console.WriteLine($"Это поток {i}");
				Console.WriteLine(res.Result.Headers);

				Thread.Sleep(5000);
			}
		}
	}
}
