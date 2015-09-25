using System.Runtime.Serialization;

namespace Mntone.NintendoNetworkHelper
{
	[DataContract]
	public sealed class AuthenticationToken
	{
		public AuthenticationToken()
		{ }

		public AuthenticationToken(string userName, string password)
		{
			this.UserName = userName;
			this.Password = password;
		}

		/// <summary>
		/// Nintendo Network ID
		/// </summary>
		[DataMember(Order = 1, Name = "username")]
		public string UserName { get; set; }

		/// <summary>
		/// Password
		/// </summary>
		[DataMember(Order = 2, Name = "password")]
		public string Password { get; set; }
	}
}
