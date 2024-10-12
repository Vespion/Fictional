using System.Runtime.CompilerServices;
using SQLitePCL;

namespace Fictional.Data.Core.Tests;

internal class Initializer
{
	[ModuleInitializer]
	internal static void SqliteLogging()
	{
		Batteries_V2.Init();
		raw.sqlite3_config_log(Log, null);
		return;

		void Log(object _, int code, string msg)
		{
			TestContext.Current.TestOutputHelper?.WriteLine($"SQLite: {code} - {msg}");
		}
	}
}