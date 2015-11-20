using System.Resources;
using System.Reflection;
using Mntone.NintendoNetworkHelper.Internal;

[assembly: AssemblyTitle(AssemblyInfo.QualifiedName)]
[assembly: AssemblyDescription(AssemblyInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AssemblyInfo.Author)]
[assembly: AssemblyProduct(AssemblyInfo.QualifiedName)]
[assembly: AssemblyCopyright("Copyright (C) 2015- " + AssemblyInfo.Author)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion("0.9.1.1")]
[assembly: AssemblyFileVersion("0.9.1.1")]

namespace Mntone.NintendoNetworkHelper.Internal
{
	internal static class AssemblyInfo
	{
		public const string Name = "Mntone.NintendoNetworkHelper";
		public const string QualifiedName = "NintendoNetworkHelper";
		public const string Description = "Nintendo Network Authorization Helper";
		public const string Version = "0.9.1.1";
		public const string Author = "mntone";
	}
}