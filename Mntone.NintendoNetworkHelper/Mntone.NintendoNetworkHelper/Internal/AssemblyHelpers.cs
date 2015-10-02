using System;

namespace Mntone.NintendoNetworkHelper.Internal
{
	internal static class AssemblyHelpers
	{
		public static Version GetAssemblyVersion(Type typeInAssembly) => new System.Reflection.AssemblyName(typeInAssembly.FullName).Version;

		public static string GetAssemblyVersionText(Type typeInAssembly)
		{
			var version = GetAssemblyVersion(typeInAssembly);
			return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
		}
	}
}