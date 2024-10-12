using System.CommandLine;
using System.CommandLine.Binding;
using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.App.ConsoleApp.Binders;

/// <inheritdoc />
internal class DatabaseBinder(Option<DirectoryInfo> directoryOption): BinderBase<SqliteConnection>
{
	private static readonly SemaphoreSlim Semaphore = new(1, 1);
	private static SqliteConnection? _connection;

	/// <inheritdoc />
	protected override SqliteConnection GetBoundValue(BindingContext bindingContext)
	{
		if(_connection != null)
		{
			return _connection;
		}

		try
		{
			Semaphore.Wait();

			if (_connection != null)
			{
				return _connection;
			}

			var directory = bindingContext.ParseResult.GetValueForOption(directoryOption);

			var filePath = Path.Combine(directory!.FullName, "graph.sqlite3");

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException();
			}

			var connectionString = new SqliteConnectionStringBuilder
			{
				DataSource = filePath
			}.ToString();

			_connection = new SqliteConnection(connectionString);

			_connection.OpenAndInitalize();

			return _connection;
		}
		finally
		{
			Semaphore.Release();
		}
	}

	internal SqliteConnection GetValue(BindingContext bindingContext)
	{
		return GetBoundValue(bindingContext);
	}
}