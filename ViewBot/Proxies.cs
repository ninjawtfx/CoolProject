using System.Collections.Generic;
using System.IO;
using xNet;

namespace ViewBot
{
	internal static class Proxies
	{
		private static List<ProxyClient> getProxiesFromFile(ProxyType type, string fileName)
		{
			List<ProxyClient> proxiesList = new List<ProxyClient>();

			foreach (var line in File.ReadAllLines(fileName))
			{
				proxiesList.Add(ProxyClient.Parse(type, line));
			}

			return proxiesList;
		}

		public static List<ProxyClient> GetProxies()
		{
			List<ProxyClient> list = new List<ProxyClient>();

			list.AddRange(getProxiesFromFile(ProxyType.Socks4, "Socks4.txt"));
			list.AddRange(getProxiesFromFile(ProxyType.Socks5, "Socks5.txt"));
			//list.AddRange(getProxiesFromFile(ProxyType.Http, "Http.txt"));

			return list;
		}
	}
}
