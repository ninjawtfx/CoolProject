using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Extreme.Net;
using static System.Console;

namespace ViewBot
{
	internal static class Proxies
	{
		public static List<ProxyClient> ProxyList = new List<ProxyClient>();
		public static List<string> GoodProxyList = new List<string>();

		private static List<ProxyClient> getProxiesFromFile(ProxyType type, string fileName)
		{
			List<ProxyClient> proxiesList = new List<ProxyClient>();

			foreach (var line in File.ReadAllLines(fileName))
			{
				var client = ProxyClient.Parse(type, line.Trim());
				if (!proxiesList.Contains(client))
					proxiesList.Add(client);

			}

			return proxiesList;
		}

		private static void WriteInFileGoodProxy()
		{
			GoodProxyList.Add(DateTime.Now.ToLongTimeString());

			File.WriteAllLines("GoodProxySocks5.txt", GoodProxyList.ToArray());
		}

		public static List<ProxyClient> GetGoodProxies()
		{
			return getProxiesFromFile(ProxyType.Http, "GoodProxyHttp.txt");
		}

		public static void GetProxies()
		{
			ProxyList.AddRange(getProxiesFromFile(ProxyType.Http, "Http.txt"));
			//list.AddRange(getProxiesFromFile(ProxyType.Socks4, "Socks4.txt"));
			//ProxyList.AddRange(getProxiesFromFile(ProxyType.Socks5, "Socks5.txt"));
		}

		public static void CheckGoodProxies()
		{
			List<Task> list = new List<Task>();

			GoodProxyList.Add(DateTime.Now.ToLongTimeString());

			for (int i = 0; i < ProxyList.Count; i++)
			{
				var lastTask = new Task(CheckProx, i);
				lastTask.Start();
				list.Add(lastTask);
			}

			Task.WaitAll(list.ToArray());

			WriteInFileGoodProxy();
		}
		private async static void CheckProx(object i)
		{
			var proxy = ProxyList[(int)i];

			var handler = new ProxyHandler(proxy);

			using (var client = new HttpClient(handler))
			{
				client.DefaultRequestHeaders.Add("Client-ID", "7sfbihg5e1b4ijo4obnsu9t4bme108");
				client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");

				try
				{
					await Program.GetToken(client);
				}
				catch (Exception)
				{
					return;
				}

				WriteLine($"Добавлена прокся {proxy.Host} - {proxy.Port}");

				GoodProxyList.Add($"{proxy.Host}:{proxy.Port}");
			}
		}
	}

		}

	

