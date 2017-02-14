using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
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

			//Parallel.ForEach(_list, GetGoodProxies);

			//Parallel.For(0, _list.Count, opt, ConnectToStreamer);

			//while (true)
			
				Parallel.ForEach(_list, opt, ConnectToStreamer);
			
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

		public async static void ConnectToStreamer(ProxyClient i)
		{
			var handler = new ProxyHandler(i);

			var request = new HttpRequestMessage();

			
			using (var client = new HttpClient(handler))
			{
				client.DefaultRequestHeaders.Add("Client-ID", "m35gcmzfmcpybvz54e2j8iaz2phxxod");

				var urlTop = string.Empty;

				var random = new Random().NextDouble();

				string es = string.Empty;

				while (true)
				{

					try
				{
					request.RequestUri = new Uri($"https://api.twitch.tv/api/channels/{_streamName}/access_token.json");
					es = await client.GetStringAsync(request.RequestUri.OriginalString);
				}

				catch (Exception)
				{
					//	if (Thread.CurrentThread.Name == null)
					return;
				}

				Token ob = JsonConvert.DeserializeObject<Token>(es);

				
					try
					{

						request.RequestUri = new Uri($"http://usher.twitch.tv/api/channel/hls/{_streamName}.m3u8?player=twitchweb" +
											 $"&token={ob.SToken}" +
											 $"&sig={ob.Sig}&allow_audio_only=true&allow_source=true" +
											 $"&type=any&p={random}");

						request.Method = HttpMethod.Get;

						var ss = await client.GetStringAsync($"http://usher.twitch.tv/api/channel/hls/{_streamName}.m3u8?player=twitchweb" +
											 $"&token={ob.SToken}" +
											 $"&sig={ob.Sig}&allow_audio_only=true&allow_source=true" +
											 $"&type=any&p={random}");

						var sss = ss.Split('#')[6].Split(new[] { "http", "\n" }, StringSplitOptions.None);

						urlTop = "http" + sss[2];

						string res = null;

						//res = await cl.SendAsync(requests);
						request.Headers.Referrer = new Uri($"https://twitch.tv/{_streamName}");
						request.RequestUri = new Uri(urlTop);

						var stri = await client.GetStringAsync(urlTop);

						

						Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}      Отправлено {i.Type} - {i.Host} - {i.Port}");

						await Task.Delay(5000);
						
						//_url = _url.Substring(0, _url.Length - 2);
				}
				catch (Exception)
				{
						return;		
					}
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
	}

