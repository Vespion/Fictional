using Fictional.Data.Core.Migrations;
using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Tests;

public class MigrationTests
{
	private SqliteConnection OpenTestConnection()
	{
		var conn = new SqliteConnection("Data Source=:memory:");
		conn.OpenAndInitalize();

		return conn;
	}


	[Fact]
	public async Task ReturnsZeroForNewDatabase()
	{
		await using var conn = OpenTestConnection();
		var mgr = new SqliteMigrationsManager(conn);

		Assert.StrictEqual(0, await mgr.GetAppliedMigration(TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task ReturnsExpectedLatestVersion()
	{
		await using var conn = OpenTestConnection();
		var mgr = new SqliteMigrationsManager(conn);

		await mgr.ApplyLatestMigrations(TestContext.Current.CancellationToken);

		Assert.StrictEqual(
			SqliteMigrationsManager.MigrationScriptReferences.Length,
			await mgr.GetLatestMigration(TestContext.Current.CancellationToken)
		);
	}

	[Fact]
	public async Task CanMigrationFromEmptyDatabase()
	{
		await using var conn = OpenTestConnection();
		var mgr = new SqliteMigrationsManager(conn);

		await mgr.ApplyLatestMigrations(TestContext.Current.CancellationToken);

		Assert.StrictEqual(
			SqliteMigrationsManager.MigrationScriptReferences.Length,
			await mgr.GetLatestMigration(TestContext.Current.CancellationToken)
		);

		Assert.StrictEqual(
			SqliteMigrationsManager.MigrationScriptReferences.Length,
			await mgr.GetAppliedMigration(TestContext.Current.CancellationToken)
		);

		Assert.StrictEqual(
			await mgr.GetLatestMigration(TestContext.Current.CancellationToken),
			await mgr.GetAppliedMigration(TestContext.Current.CancellationToken)
		);
	}
}