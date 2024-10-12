namespace Fictional.Data.Core.Migrations;

/// <summary>
///     Manages the migrations of the database.
/// </summary>
public interface IMigrationsManager
{
	/// <summary>
	///     Gets the version of the latest applied migration.
	/// </summary>
	/// <param name="cancellationToken">An optional cancellation token</param>
	/// <returns>The version code of the latest applied migration</returns>
	ValueTask<int> GetAppliedMigration(CancellationToken cancellationToken = default);

	/// <summary>
	///     Gets the version of the latest migration.
	/// </summary>
	/// <param name="cancellationToken">An optional cancellation token</param>
	/// <returns>The version code of the latest un-applied migration</returns>
	ValueTask<int> GetLatestMigration(CancellationToken cancellationToken = default);

	/// <summary>
	///     Applies the latest migrations to the database.
	/// </summary>
	/// <remarks>
	///     Upon completion of this method <see cref="GetAppliedMigration" /> and <see cref="GetLatestMigration" /> will
	///     return the same version code
	/// </remarks>
	/// <param name="cancellationToken">An optional cancellation token</param>
	/// <returns>A <see cref="ValueTask" /> that completes when the migration is complete</returns>
	ValueTask ApplyLatestMigrations(CancellationToken cancellationToken = default);
}