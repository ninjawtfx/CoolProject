using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ViewBot
{
	class myWebProxy : IWebProxy
	{
		private IWebProxy wrappedProxy;
		private ICredentials creds;
		private void init()
		{
			wrappedProxy = null;
			creds = CredentialCache.DefaultCredentials;
		}
		public myWebProxy()
		{
			init();
		}

		public myWebProxy(IWebProxy theWrappedProxy)
		{
			init();
			wrappedProxy = theWrappedProxy;
		}
		public ICredentials Credentials
		{
			get
			{
				if (wrappedProxy != null)
				{
					return wrappedProxy.Credentials;
				}
				else
				{
					return creds;
				}
			}
			set
			{
				if (wrappedProxy != null)
				{
					wrappedProxy.Credentials = value;
				}
				else
				{
					creds = value;
				}

			}
		}

		public Uri GetProxy(Uri destination)
		{
			if (wrappedProxy != null /* todo or Uri == certain Uri */)
			{
				return wrappedProxy.GetProxy(destination);
			}
			else
			{
				// hardcoded proxy here..
				return new Uri("http://seeplusplus:8080");
			}
		}

		public bool IsBypassed(Uri host)
		{
			if (wrappedProxy != null)
			{
				return wrappedProxy.IsBypassed(host);
			}
			else
			{
				return false;
			}

		}
	}
}
