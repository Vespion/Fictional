﻿using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.GraphTests;

public class FetchTests
{
	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	public async Task FetchesTagChildren(SerializableTag[] tags)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var canonicalId = tags[0].Id;

		for (var i = 1; i < tags.Length; i++)
			conn.ExecuteNonQuery(
				$"INSERT INTO TagGraphLinks (ParentTagId, ChildTagId) VALUES ({canonicalId.Value}, {tags[i].Id.Value})");

		var aliasIds = await repo.GetChildrenAsync(canonicalId, TestContext.Current.CancellationToken);

		aliasIds.Should().BeEquivalentTo(tags.Skip(1).Select(t => t.Id));
	}

	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	public async Task FetchesRecursiveTagChildren(SerializableTag[] tags)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var canonicalId = tags[0].Id;

		for (var i = 1; i < tags.Length - 1; i++)
		{
			conn.ExecuteNonQuery(
				$"INSERT INTO TagGraphLinks (ParentTagId, ChildTagId) VALUES ({canonicalId.Value}, {tags[i].Id.Value})");
			canonicalId = tags[i].Id;
		}

		var aliasIds = await repo.GetChildrenAsync(tags[0].Id, TestContext.Current.CancellationToken);

		aliasIds.Should().BeEquivalentTo(tags.Skip(1).SkipLast(1).Select(t => t.Id));
	}
}