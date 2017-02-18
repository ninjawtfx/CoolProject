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
using Extreme.Net;
using System.Net;

namespace ViewBot
{
	class Program
	{
		private static string _url;
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

			//Parallel.For(0, _list.Count, opt, ConnectToStreamer);

			//	Parallel.ForEach(list, ConnectToStreamer);

			for (int i = 0; i < _list.Count; i++)
			{
				ThreadPool.QueueUserWorkItem(ConnectToStreamer, i);
			}

			//	var task = new Thread(ConnectToStreamer);
				
			//	try
			//	{
			//		task.Start(i);
			//	}
			//	catch (Exception)
			//	{
			//		Console.WriteLine($"Dispose {task.ManagedThreadId}");
			//		task.Interrupt();
			//	}
			//}


			Console.Read();
		}

		public async static void ConnectToStreamer(object j)
		{
			int i = (int)j;

			var client = _list[(int)i];

			var livestreamerPath = $"{ConfigurationManager.AppSettings["LivestreamerPath"]}livestreamer.exe";

			Process cmd = new Process();
			cmd.StartInfo.FileName = livestreamerPath;
			cmd.StartInfo.Arguments = $"--http-proxy http://{client.Host}:{client.Port} --http-header Client-ID=m35gcmzfmcpybvz54e2j8iaz2phxxod {_streamName} -j";

			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();

			StreamData ob = JsonConvert.DeserializeObject<StreamData>(cmd.StandardOutput.ReadToEnd());

			string url;
			try
			{
				url = ob.Streams.Audio.Url;
			}

			catch(Exception)
			{
				cmd.Dispose();
				return;
			}
			cmd.Dispose();

			Stopwatch sw = new Stopwatch();

			sw.Start();

			while (true)
			{
				if (sw.Elapsed.Seconds > 55)
				{
					cmd = new Process();
					cmd.StartInfo.FileName = livestreamerPath;
					cmd.StartInfo.Arguments = $"--http-proxy http://{client.Host}:{client.Port} --http-header Client-ID=m35gcmzfmcpybvz54e2j8iaz2phxxod {_streamName} -j";

					cmd.StartInfo.RedirectStandardInput = true;
					cmd.StartInfo.RedirectStandardOutput = true;
					cmd.StartInfo.CreateNoWindow = true;
					cmd.StartInfo.UseShellExecute = false;
					cmd.Start();

					ob = JsonConvert.DeserializeObject<StreamData>(cmd.StandardOutput.ReadToEnd());
										
					try
					{
						url = ob.Streams.Audio.Url;
					}

					catch (Exception)
					{
						cmd.Dispose();
						return;
					}
					cmd.Dispose();
					sw.Reset();
				}

				ProxyHandler handler = new ProxyHandler(client);

				using (var request = new HttpClient(handler))
				{
					//HttpRequestMessage requests = new HttpRequestMessage();
					//requests.RequestUri = new Uri(ob.Streams.Audio.Url);
					//requests.Method = System.Net.Http.HttpMethod.Get;

					//HttpClientHandler hand = new HttpClientHandler
					//{
					//	UseProxy = true,
					//	Proxy = new WebProxy($"{client.Host}:{client.Port}")
					//};

					//HttpClient cl = new HttpClient(hand);

					string res;

					try
					{
						//res = await cl.SendAsync(requests);
						res = await request.GetStringAsync(url);

						var firstPart = url.Split(new string[] { "index" }, StringSplitOptions.None)[0];
						var secondPart = res.Split('#')[14].Split(new string [] { "index" }, StringSplitOptions.None)[1];
						var newUrl = firstPart + "index" + secondPart;

						res = await request.GetStringAsync(newUrl);

						Console.WriteLine($"Отправлено {client.Type} - {client.Host}");
					}
					catch (Exception ex)
					{
						//	//_logger.Trace(ex);
						//return;
					}

					
				}
				
				Thread.Sleep(5000);
			} 
		}
	}
}
