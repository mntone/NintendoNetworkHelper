namespace Mntone.NintendoNetworkHelper
{
	public sealed class AccessToken
	{
		internal AccessToken(string userName, string clientID, string sessionValue)
		{
			this.UserName = userName;
			this.ClientID = clientID;
			this.SessionValue = sessionValue;
		}

		public string UserName { get; }
		public string ClientID { get; }
		public string SessionValue { get; }
	}
}