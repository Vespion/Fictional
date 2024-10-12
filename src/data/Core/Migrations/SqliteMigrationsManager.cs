using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Migrations;

/// <summary>
///     Implementation of <see cref="IMigrationsManager" /> for SQLite.
/// </summary>
/// <param name="connection">The SQLite connection</param>
public class SqliteMigrationsManager(SqliteConnection connection) : IMigrationsManager
{
	internal static readonly EmbeddedResource[] MigrationScriptReferences =
	[
		EmbeddedResource.Migrations_Scripts_00_Initial_sql
	];

	/// <inheritdoc />
	public ValueTask<int> GetAppliedMigration(CancellationToken cancellationToken = default)
	{
		var version = connection.ExecuteScalar<long?>("PRAGMA user_version") ?? 0;
		return ValueTask.FromResult((int)version);
	}

	/// <inheritdoc />
	public ValueTask<int> GetLatestMigration(CancellationToken cancellationToken = default)
	{
		return new ValueTask<int>(MigrationScriptReferences.Length);
	}

	/// <inheritdoc />
	public async ValueTask ApplyLatestMigrations(CancellationToken cancellationToken = default)
	{
		var currentVersion = await GetAppliedMigration(cancellationToken);

		foreach (var resource in MigrationScriptReferences.Skip(currentVersion))
		{
			using var reader = resource.GetReader();
			var script = await reader.ReadToEndAsync(cancellationToken);
			connection.ExecuteNonQuery(script);
		}

		var cmd = $"PRAGMA user_version = {MigrationScriptReferences.Length}";
		connection.ExecuteNonQuery(cmd);
	}
}