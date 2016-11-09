using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using NLog;
using xNet;

namespace ViewBot
{
	class Program
	{
		private static string _urlRequest;
		private static string _streamName;

		private static Logger _logger;

		static void Main(string[] args)
		{
			var livestreamerPath = ConfigurationManager.AppSettings["LivestreamerPath"];
			_streamName = $"twitch.tv/{ConfigurationManager.AppSettings["StreamName"]}";

			_logger = LogManager.GetCurrentClassLogger();

			Process cmd = new Process();
			cmd.StartInfo.FileName = "livestreamer.exe";
			cmd.StartInfo.Arguments = $"--http-header Client-ID=ewvlchtxgqq88ru9gmfp1gmyt6h2b93 {_streamName} -j";

			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();

			StreamData ob = JsonConvert.DeserializeObject<StreamData>(cmd.StandardOutput.ReadToEnd());

			_urlRequest = ob.Streams.Audio.Url;

			var list = Proxies.GetProxies();

			Parallel.ForEach(list, ConnectToStreamer); 

			Console.Read();
		}

		public static void ConnectToStreamer(ProxyClient client)
		{
			while (true)
			{
				using (var request = new HttpRequest())
				{
					request.UserAgent = Http.ChromeUserAgent();
					request.Proxy = client;

					HttpResponse res;

					try
					{
						res = request.Get(_urlRequest);
					}
					catch (Exception ex)
					{
						_logger.Trace(ex);
						return;
					}
					
					Console.WriteLine($"Это proxy {client.Host}");
					Console.WriteLine(res.IsOK);

				}
			}
		}
	}
}
