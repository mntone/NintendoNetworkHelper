﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mntone.NintendoNetworkHelper.Internal;
using System.Net;

namespace Mntone.NintendoNetworkHelper
{
	public sealed class NintendoNetworkAuthorizer : IDisposable
	{
		private const string AUTHORIZE_URI = "https://id.nintendo.net/oauth/authorize";

		private bool _disposed = false;
		private bool _isUwp = false;

		public Task<RequestToken> GetRequestTokenAsync(string requestTokenUriText, HttpContent value = null)
			=> this.GetRequestTokenAsync(new Uri(requestTokenUriText), value);

		public Task<RequestToken> GetRequestTokenAsync(Uri requestTokenUri, HttpContent value = null)
		{
			return this.Client.PostAsync(requestTokenUri, value ?? new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()))
			.ContinueWith(r =>
			{
				var result = r.Result;
				if (result.StatusCode != HttpStatusCode.Found)
				{
					throw new NintendoNetworkException(Messages.CannotGetRequestToken);
				}

				var location = result.Headers.Location;
				var queries = location.QueryToKeyValuePair();

				string clientID = null, responseType = null, redirectUriText = null, state = null;
				foreach (var query in queries)
				{
					switch (query.Key)
					{
						case "client_id":
							clientID = query.Value;
							break;

						case "response_type":
							responseType = query.Value;
							break;

						case "redirect_uri":
							redirectUriText = query.Value;
							break;

						case "state":
							state = query.Value;
							break;
					}
				}

				if (clientID == null || responseType == null || redirectUriText == null || state == null)
				{
					throw new NintendoNetworkException(Messages.CannotGetRequestToken);
				}

				return new RequestToken(clientID, responseType, redirectUriText, state);
			});
		}

		public Task<AccessToken> Authorize(RequestToken requestToken, AuthenticationToken authenticationToken, Func<CookieContainer, string> sessionValueGetter)
		{
			return this.Client.PostAsync(AUTHORIZE_URI, new FormUrlEncodedContent(new Dictionary<string, string>()
			{
				["client_id"] = requestToken.ClientID,
				["response_type"] = requestToken.ResponseType,
				["redirect_uri"] = requestToken.RedirectUri.ToString(),
				["state"] = requestToken.State,
				["nintendo_authenticate"] = string.Empty,
				["nintendo_authorize"] = string.Empty,
				["scope"] = string.Empty,
				["lang"] = "ja-JP",
				["username"] = authenticationToken.UserName,
				["password"] = authenticationToken.Password,
			})).ContinueWith(r =>
			{
				var result = r.Result;
				if (result.StatusCode == HttpStatusCode.OK)
				{
					var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					if (content.Contains("hb-error-wrapper")) throw new NintendoNetworkException(Messages.NnidOrPasswordIsIncorrect);
					throw new NintendoNetworkException(Messages.CannotGetAccessToken);
				}
				if (result.StatusCode != HttpStatusCode.RedirectMethod)
				{
					throw new NintendoNetworkException(Messages.CannotGetAccessToken);
				}
				return this.Client.GetAsync(result.Headers.Location);
			}).Unwrap()
			.ContinueWith(r =>
			{
				var result = r.Result;
				if (result.StatusCode != HttpStatusCode.Found)
				{
					throw new NintendoNetworkException(Messages.CannotGetAccessToken);
				}

				var sessionValue = this._isUwp ? HttpClientHelper.ProcessResponseCookies(result, sessionValueGetter) : sessionValueGetter(this._clientHandler.CookieContainer);
				if (sessionValueGetter == null)
				{
					throw new NintendoNetworkException(Messages.CannotGetAccessToken);
				}
				return new AccessToken(authenticationToken.UserName, requestToken.ClientID, sessionValue);
			});
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this._disposed) return;
			if (disposing) this.Release();
			this._disposed = true;
		}

		private void Release()
		{
			if (this._Client != null)
			{
				this._Client.Dispose();
				this._clientHandler = null;
				this._Client = null;
			}
		}

		public HttpClient Client
		{
			get
			{
				if (this._Client == null)
				{
					this._clientHandler = new HttpClientHandler()
					{
						AllowAutoRedirect = false,
						AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
					};
					this._isUwp = HttpClientHelper.IsUwpHttpClient(this._clientHandler);
					this._Client = new HttpClient(this._clientHandler);
					this._Client.DefaultRequestHeaders.Add("user-agent",
						!string.IsNullOrEmpty(this.AdditionalUserAgent)
							? $"{AssemblyInfo.QualifiedName}/{AssemblyInfo.Version} ({this.AdditionalUserAgent})"
							: $"{AssemblyInfo.QualifiedName}/{AssemblyInfo.Version}");
				}
				return this._Client;
			}
		}
		public HttpClientHandler _clientHandler = null;
		private HttpClient _Client = null;

		public string AdditionalUserAgent
		{
			get { return this._AdditionalUserAgent; }
			set
			{
				if (this._AdditionalUserAgent == value) return;

				this._AdditionalUserAgent = value;
				this.Release();
			}
		}
		private string _AdditionalUserAgent = null;
	}
}
