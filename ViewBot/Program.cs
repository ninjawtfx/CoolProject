using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json;
using NLog;
using xNet;

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
			_streamName = ConfigurationManager.AppSettings["StreamName"];

			_logger = LogManager.GetCurrentClassLogger();

			var opt = new ParallelOptions();
			opt.MaxDegreeOfParallelism = 1000;

			_list = Proxies.GetProxies();

			//Parallel.For(0, _list.Count, opt, ConnectToStreamer);

			//Parallel.ForEach(_list, ConnectToStreamer);

			for (int i = 0; i < _list.Count; i++)
			{
				var task = new Thread(ConnectToStreamer);

				try
				{
					task.Start(i);
				}
				catch (Exception)
				{
					Console.WriteLine($"Dispose {task.ManagedThreadId}");
					task.Interrupt();
				}
			}


			Console.Read();
		}

		public static void ConnectToStreamer(object i)
		{
			using (var request = new HttpRequest())
			{
				var client = _list[(int)i];

				request.Proxy = client;

				request.AddHeader("Client-ID", "m35gcmzfmcpybvz54e2j8iaz2phxxod");

				var random = new Random().NextDouble();

				try
				{
					var es = request.Get($"https://api.twitch.tv/api/channels/{_streamName}/access_token.json").ToString();

					Token ob = JsonConvert.DeserializeObject<Token>(es);

					var ss = request.Get($"http://usher.twitch.tv/api/channel/hls/{_streamName}.m3u8?player=twitchweb" +
										 $"&token={ob.SToken}" +
										 $"&sig={ob.Sig}&allow_audio_only=true&allow_source=true" +
										 $"&type=any&p={random}").ToString();

					var sss = ss.Split('#')[4].Split(new[] { "http" }, StringSplitOptions.None);

					_url = "http" + sss[1];
					//_url = _url.Substring(0, _url.Length - 2);
				}
				catch (Exception)
				{
					return;
				}
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

				while (true)
				{


					HttpResponse res;

					try
					{
						//res = await cl.SendAsync(requests);
						res = request.Get(_url);
						Thread.Sleep(5000);
					}
					catch (Exception ex)
					{
						//	//_logger.Trace(ex);
						//return;
					}

					Console.WriteLine($"Отправлено {client.Type} - {client.Host}");
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
}
