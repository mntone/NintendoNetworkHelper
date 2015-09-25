using System;

namespace Mntone.NintendoNetworkHelper
{
	public sealed class RequestToken
	{
		internal RequestToken(string clientID, string responseType, string redirectUriText, string state)
		{
			this.ClientID = clientID;
			this.ResponseType = responseType;
			this.RedirectUri = new Uri(Uri.UnescapeDataString(redirectUriText));
			this.State = state;
		}

		public string ClientID { get; }
		public string ResponseType { get; }
		public Uri RedirectUri { get; }
		public string State { get; }
	}
}