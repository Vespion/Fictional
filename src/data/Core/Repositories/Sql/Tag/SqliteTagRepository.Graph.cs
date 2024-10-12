using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Sql;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Repositories.Sql;

public partial class SqliteTagRepository
{
	/// <inheritdoc />
	public ValueTask<bool> AddGraphLinkAsync(TagId parentTag, TagId childTag,
		CancellationToken cancellationToken = default)
	{
		try
		{
			connection.ExecuteNonQuery(
				$"INSERT INTO TagGraphLinks (ParentTagId, ChildTagId) VALUES ({parentTag.Value}, {childTag.Value})");
		}
		catch (SqliteException e) when (e.SqliteErrorCode == 19)
		{
			// Ignore duplicate key errors
			return new ValueTask<bool>(false);
		}

		return new ValueTask<bool>(true);
	}

	/// <inheritdoc />
	public ValueTask<bool> RemoveGraphLinkAsync(TagId parentTag, TagId childTag,
		CancellationToken cancellationToken = default)
	{
		var deletedRow = connection.ExecuteNonQuery(
			                 $"DELETE FROM TagGraphLinks WHERE ParentTagId = {parentTag.Value} AND ChildTagId = {childTag.Value}")
		                 == 1;
		return new ValueTask<bool>(deletedRow);
	}

	/// <inheritdoc />
	public async ValueTask<IEnumerable<TagId>> GetChildrenAsync(TagId parentTag,
		CancellationToken cancellationToken = default)
	{
		return await Task.Run(() => connection.Query<TagId>(
			$"""
			 WITH RECURSIVE children_of(tag) AS (
			     SELECT ({parentTag.Value})
			     UNION ALL
			     SELECT TagGraphLinks.ChildTagId
			     FROM TagGraphLinks, children_of
			     WHERE TagGraphLinks.ParentTagId = children_of.tag
			 )
			 SELECT Id FROM Tags, children_of WHERE Tags.Id = children_of.tag AND Tags.Id != {parentTag.Value};
			 """), cancellationToken);
	}
}