using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Repositories.Sql;

public partial class SqliteTagRepository
{
	/// <inheritdoc />
	public ValueTask<bool> AddAliasLinkAsync(TagId canonicalTag, TagId aliasTag,
		CancellationToken cancellationToken = default)
	{
		try
		{
			connection.ExecuteNonQuery(
				$"INSERT INTO TagAliasLinks (CanonicalTagId, AliasTagId) VALUES ({canonicalTag.Value}, {aliasTag.Value})");
		}
		catch (SqliteException e) when (e.SqliteErrorCode == 19)
		{
			// Ignore duplicate key errors
			return new ValueTask<bool>(false);
		}

		return new ValueTask<bool>(true);
	}

	/// <inheritdoc />
	public ValueTask<bool> RemoveAliasLinkAsync(TagId canonicalTag, TagId aliasTag,
		CancellationToken cancellationToken = default)
	{
		var deletedRow = connection.ExecuteNonQuery(
			                 $"DELETE FROM TagAliasLinks WHERE CanonicalTagId = {canonicalTag.Value} AND AliasTagId = {aliasTag.Value}")
		                 == 1;
		return new ValueTask<bool>(deletedRow);
	}

	/// <inheritdoc />
	public async ValueTask<IEnumerable<TagId>> GetAliasesAsync(TagId canonicalTag,
		CancellationToken cancellationToken = default)
	{
		return await Task.Run(() => connection.Query<TagId>(
			$"""
			 WITH RECURSIVE children_of(tag) AS (
			     SELECT ({canonicalTag.Value})
			     UNION ALL
			     SELECT TagAliasLinks.AliasTagId
			     FROM TagAliasLinks, children_of
			     WHERE TagAliasLinks.CanonicalTagId = children_of.tag
			 )
			 SELECT Id FROM Tags, children_of WHERE Tags.Id = children_of.tag AND Tags.Id != {canonicalTag.Value};
			 """), cancellationToken);
	}
}