using System;

namespace Mntone.NintendoNetworkHelper
{
	public sealed class NintendoNetworkException : Exception
	{
		public NintendoNetworkException(string message)
			: base(message)
		{ }
	}
}