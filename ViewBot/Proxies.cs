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

		public static void GetProxies()
		{
			ProxyList.AddRange(getProxiesFromFile(ProxyType.Http, "Http.txt"));
			//list.AddRange(getProxiesFromFile(ProxyType.Socks4, "Socks4.txt"));
			//list.AddRange(getProxiesFromFile(ProxyType.Socks5, "Socks5.txt"));
		}

		public static List<ProxyClient> GetGoodProxies()
		{
			var list = new List<ProxyClient>();

			foreach (var proxy in ProxyList)
			{
				Task.Run(async () =>
				{
					var handler = new ProxyHandler(proxy);

					using (var client = new HttpClient(handler))
					{
						try
						{
							await Program.GetToken(client);
						}
						catch (Exception)
						{
							return;
						}

						WriteLine($"Добавлена прокся {proxy.Host} - {proxy.Port}");

						list.Add(proxy);
					}
				});
			}

			return list;
		}
	}
}
