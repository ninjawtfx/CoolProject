using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using xNet;
using HttpMethod = xNet.HttpMethod;

namespace ViewBot
{
	class Program
	{
		private static string _streamName;

		private static Logger _logger;

		private static List<ProxyClient> _list;

		static void Main(string[] args)
		{
			_streamName = $"twitch.tv/{ConfigurationManager.AppSettings["StreamName"]}";

			_logger = LogManager.GetCurrentClassLogger();
			
			_list = Proxies.GetProxies();

			var opt = new ParallelOptions();
			opt.MaxDegreeOfParallelism = 1000;

			Parallel.For(0, _list.Count, opt, ConnectToStreamer);

		//	Parallel.ForEach(list, ConnectToStreamer);

			Console.Read();
		}

		public static async void ConnectToStreamer(int i)
		{
			var livestreamerPath = $"{ConfigurationManager.AppSettings["LivestreamerPath"]}livestreamer.exe";

			Process cmd = new Process();
			cmd.StartInfo.FileName = livestreamerPath;
			cmd.StartInfo.Arguments = $"--http-header Client-ID=m35gcmzfmcpybvz54e2j8iaz2phxxod {_streamName} -j";

			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();

			StreamData ob = JsonConvert.DeserializeObject<StreamData>(cmd.StandardOutput.ReadToEnd());

			while (true)
			{
				using (var request = new HttpRequest())
				{
					var client = _list[i];

					request.Proxy = client;

					HttpRequestMessage requests = new HttpRequestMessage();
					requests.RequestUri = new Uri(ob.Streams.Audio.Url);
					requests.Method = System.Net.Http.HttpMethod.Head;

					HttpClientHandler hand = new HttpClientHandler
					{
						UseProxy = true,
						Proxy = new WebProxy($"{client.Host}:{client.Port}")
					};

					HttpClient cl = new HttpClient(hand);
					
					HttpResponseMessage res;

					try
					{
						res = await cl.SendAsync(requests);
						//res = request.Get(ob.Streams.Audio.Url).ToString();
					}
					catch (Exception ex)
					{
						_logger.Trace(ex);
						return;
					}

					if (res.Headers.ConnectionClose != null && (bool) res.Headers.ConnectionClose)
					{
						return;
					}
					
					Console.WriteLine($"Это proxy {client.Type}  --  {client.Host}:{client.Port}");
					Console.WriteLine(res.Headers);
				}

				Thread.Sleep(50000);
			}
		}
	}
}
