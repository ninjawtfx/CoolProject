using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using NLog;
using Extreme.Net;
using Extreme;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.String;

namespace ViewBot
{
	class Program
	{
		private static List<string> listAddedUrl = new List<string>();

		private static string _url;
		private static string _streamName;

		private static Logger _logger;

		private static List<ProxyClient> _list;
		private static List<ProxyClient> _tryList;

		static void Main(string[] args)
		{
			Thread.CurrentThread.Name = "Main";
			_streamName = ConfigurationManager.AppSettings["StreamName"];

			_logger = LogManager.GetCurrentClassLogger();

			var opt = new ParallelOptions();
			opt.MaxDegreeOfParallelism = 5000;

			_list = Proxies.GetProxies();
			_tryList = new List<ProxyClient>();

			int nWorkerThreads;
			int nCompletionThreads;
			ThreadPool.GetMaxThreads(out nWorkerThreads, out nCompletionThreads);

			Console.WriteLine($"{nWorkerThreads} --- {nCompletionThreads}");

			//ThreadPool.SetMaxThreads(200, 200);
			//ThreadPool.SetMinThreads(1000, 1000);

			while (true)
			{
				for (int i = 0; i < _list.Count; i++)
					ThreadPool.QueueUserWorkItem(ConnectToStreamer, i);

				Thread.Sleep(10000);
			}
			//Parallel.ForEach(_list, GetGoodProxies);

			//Parallel.For(0, _list.Count, opt, ConnectToStreamer);

			//while (true)

			//Parallel.ForEach(_list, opt, ConnectToStreamer);

			//foreach(var proxy in _list)
			//{ 
			//	var task = new Thread(ConnectToStreamer);

			//	task.Start(proxy);
			//}


			Console.Read();
		}


		#region livestreamer

		///var livestreamerPath = $"{ConfigurationManager.AppSettings["LivestreamerPath"]}livestreamer.exe";

		//request.Proxy = client;

		//HttpRequestMessage requests = new HttpRequestMessage();
		//requests.RequestUri = new Uri(ob.Streams.Audio.Url);
		//requests.Method = System.Net.Http.HttpMethod.Get;

		//HttpClientHandler hand = new HttpClientHandler
		//{
		//	UseProxy = true,
		//	Proxy = new WebProxy($"{client.Host}:{client.Port}")
		//};

		//HttpClient cl = new HttpClient(hand);

		#endregion

		public static async Task<Token> GetToken(HttpClient client)
		{
			string es = await client.GetStringAsync($"https://api.twitch.tv/api/channels/{_streamName}/access_token.json");

			return JsonConvert.DeserializeObject<Token>(es);
		}

		private static readonly DateTime Jan1st1970 = new DateTime
			(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public static async void ConnectToStreamer(object j)
		{
			ProxyClient i = _list[(int) j];

			var handler = new ProxyHandler(i);

			var request = new HttpRequestMessage(HttpMethod.Head, "");


			using (var client = new HttpClient(handler))
			{

				client.DefaultRequestHeaders.Add("Client-ID", "7sfbihg5e1b4ijo4obnsu9t4bme108");
				client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");

				var random = new Random().NextDouble();

				string es = string.Empty;

				Token ob;

				try
				{
					ob = await GetToken(client);
				}

				catch (Exception)
				{
					return;
				}

				var sw = new Stopwatch();

				sw.Start();

				while (true)
				{

					try
					{
						var ss = CurrentTimeMillis() / 1000;
						
						if (sw.Elapsed.Minutes > 1)
						{
							ob = await GetToken(client);
							sw.Reset();
						}
					}
					catch (Exception)
					{
						return;
					}

					try
					{

						var urlTop = string.Empty;

						request.RequestUri = new Uri($"http://usher.twitch.tv/api/channel/hls/{_streamName}.m3u8?player=twitchweb" +
														$"&token={ob.SToken}" +
														$"&sig={ob.Sig}&allow_audio_only=true" +
														$"&type=any&p={random}");

						var ss = await client.GetStringAsync($"http://usher.twitch.tv/api/channel/hls/{_streamName}.m3u8?player=twitchweb" +
											 $"&token={ob.SToken}" +
											 $"&sig={ob.Sig}&allow_audio_only=true&allow_source=true" +
											 $"&type=any&p={random}");

						var sss = ss.Split('#')[14].Split(new[] {"http", "\n"}, StringSplitOptions.None);

						urlTop = "http" + sss[2];

						string res = null;

						//res = await cl.SendAsync(requests);
						request.Headers.Referrer = new Uri($"https://twitch.tv/{_streamName}");
						request.RequestUri = new Uri(urlTop);

						HttpResponseMessage stri;

						
							using (var req = new HttpRequestMessage(HttpMethod.Head, urlTop))
							{
								stri = await client.SendAsync(req);
							}
						
						Console.WriteLine($"{stri.StatusCode} {Thread.CurrentThread.ManagedThreadId}      Отправлено {i.Type} - {i.Host} - {i.Port}");

						await Task.Delay(5000);

						//_url = _url.Substring(0, _url.Length - 2);
					}

					catch (Exception ex)
					{
						if (ex is ProxyException)
							return;
					}
				}



			}



			//Process cmd = new Process();
			//cmd.StartInfo.FileName = livestreamerPath;
			//cmd.StartInfo.Arguments = $"--http-header Client-ID=m35gcmzfmcpybvz54e2j8iaz2phxxod {_streamName} -j";

			//cmd.StartInfo.RedirectStandardInput = true;
			//cmd.StartInfo.RedirectStandardOutput = true;
			//cmd.StartInfo.CreateNoWindow = true;
			//cmd.StartInfo.UseShellExecute = false;
			//cmd.Start();

			//StreamData ob = JsonConvert.DeserializeObject<StreamData>(cmd.StandardOutput.ReadToEnd());

			//_url = ob.Streams.Audio.Url;
		}
		private static object _lock = new object();
	}

	

}
	

