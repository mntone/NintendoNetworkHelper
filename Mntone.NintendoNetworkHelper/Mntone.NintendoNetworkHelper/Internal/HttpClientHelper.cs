using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Mntone.NintendoNetworkHelper.Internal
{
	internal sealed class HttpClientHelper
	{
		public static bool IsUwpHttpClient(HttpClientHandler handler)
		{
			if (handler.MaxAutomaticRedirections != 10) return false;
			try
			{
				handler.MaxAutomaticRedirections = 11;
			}
			catch (PlatformNotSupportedException)
			{
				return true;
			}

			return false;
		}

		public static string ProcessResponseCookies(HttpResponseMessage response, Func<CookieContainer, string> sessionValueGetter)
		{
			IEnumerable<string> cookies = null;
			if (response.Headers.TryGetValues("Set-Cookie", out cookies))
			{
				foreach (var item in cookies)
				{
					if (!string.IsNullOrWhiteSpace(item))
					{
						try
						{
							var requestUri = response.RequestMessage.RequestUri;
							var temporaryCookieContainer = new CookieContainer();
							temporaryCookieContainer.SetCookies(requestUri, item);

							var sessionValue = sessionValueGetter(temporaryCookieContainer);
							if (sessionValue != null) return sessionValue;
						}
						catch (Exception) { }
					}
				}
			}
			return null;
		}
	}
}
