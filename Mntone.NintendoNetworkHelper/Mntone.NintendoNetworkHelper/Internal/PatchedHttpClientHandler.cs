using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Mntone.NintendoNetworkHelper.Internal
{
	internal sealed class PatchedHttpClientHandler : MessageProcessingHandler
	{
		public static HttpMessageHandler PatchOrDefault(HttpClientHandler handler)
		{
			if (handler.MaxAutomaticRedirections != 10) return handler;
			try
			{
				handler.MaxAutomaticRedirections = 11;
			}
			catch (PlatformNotSupportedException)
			{
				return new PatchedHttpClientHandler(handler);
			}

			return handler;
		}

		private bool _disposed = false;
		private HttpClientHandler _innerHandler;

		public PatchedHttpClientHandler(HttpClientHandler handler)
			: base(handler)
		{
			this._innerHandler = handler;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this._disposed)
			{
				this._disposed = true;
				if (this._innerHandler != null)
				{
					this._innerHandler.Dispose();
					this._innerHandler = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return request;
		}

		protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			this.ProcessResponseCookies(response);
			return response;
		}

		private void ProcessResponseCookies(HttpResponseMessage response)
		{
			IEnumerable<string> cookies = null;
			if (this._innerHandler.UseCookies && response.Headers.TryGetValues("Set-Cookie", out cookies))
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
							foreach (var cookie in temporaryCookieContainer.GetCookies(requestUri).Cast<Cookie>())
							{
								if (cookie.HttpOnly)
								{
									this._innerHandler.CookieContainer.Add(response.RequestMessage.RequestUri, cookie);
								}
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}
	}
}