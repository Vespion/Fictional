using System.Runtime.CompilerServices;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Repositories.Sql;

/// <inheritdoc />
/// <summary>
///     A repository for <see cref="Tag" /> entities that uses SQLite.
/// </summary>
public partial class SqliteTagRepository(SqliteConnection connection) : ITagRepository
{
	/// <inheritdoc />
	public ValueTask<Tag?> GetByIdAsync(TagId id, CancellationToken cancellationToken = default)
	{
		var tag = connection.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {id.Value} LIMIT 1");

		if (tag == default) return ValueTask.FromResult<Tag?>(null);

		return ValueTask.FromResult<Tag?>(tag);
	}

	/// <inheritdoc />
#pragma warning disable CS1998 // SQLite doesn't support async enumeration anyway so this is fine
	public async IAsyncEnumerable<Tag> GetAllAsync(
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998
	{
		foreach (var tag in connection.Query<Tag>("SELECT * FROM Tags"))
		{
			yield return tag;

			if (cancellationToken.IsCancellationRequested) yield break;
		}
	}

	/// <inheritdoc />
	public ValueTask AddAsync(Tag obj, CancellationToken cancellationToken = default)
	{
		connection.ExecuteNonQuery(
			$"INSERT INTO Tags (Id, Name, Shorthand, Colour, Hidden) VALUES ({obj.Id.Value}, {obj.Name}, {obj.Shorthand}, {(obj.Colour != null ? (int?)obj.Colour : null)}, {obj.Hidden})");

		return ValueTask.CompletedTask;
	}

	/// <inheritdoc />
	public ValueTask<bool> ExistsAsync(TagId id, CancellationToken cancellationToken = default)
	{
		var returnValue = connection.ExecuteScalar<long?>(
			$"SELECT EXISTS(SELECT 1 FROM Tags WHERE Id = {id.Value});") ?? 0;
		return ValueTask.FromResult(returnValue == 1);
	}

	/// <inheritdoc />
	public ValueTask DeleteAsync(TagId id, CancellationToken cancellationToken = default)
	{
		connection.ExecuteNonQuery($"DELETE FROM Tags WHERE Id = {id.Value}");
		return ValueTask.CompletedTask;
	}

	/// <inheritdoc />
	public ValueTask UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
	{
		var effected = connection.ExecuteNonQuery($"UPDATE Tags SET Name = {tag.Name}, Shorthand = {tag.Shorthand}, Colour = {(tag.Colour != null ? (int?)tag.Colour : null)}, Hidden = {tag.Hidden} WHERE Id = {tag.Id.Value}");

		if (effected == 0) throw new KeyNotFoundException();

		return ValueTask.CompletedTask;
	}
}