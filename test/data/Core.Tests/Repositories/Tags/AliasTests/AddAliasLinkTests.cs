using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.AliasTests;

public class AddAliasLinkTests
{
	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task AddsAliasToTag(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var repo = new SqliteTagRepository(conn);

		(await repo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken)).Should()
			.BeTrue();

		var linkExists =
			conn.ExecuteScalar<long>(
				$"SELECT COUNT(*) FROM TagAliasLinks WHERE AliasTagId = {aliasTag.Id.Value} AND CanonicalTagId = {canonicalTag.Id.Value}") ==
			1;

		linkExists.Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task IgnoresAddingDuplicateAliasToTag(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var repo = new SqliteTagRepository(conn);

		(await repo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken)).Should()
			.BeTrue();

		(await repo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken)).Should()
			.BeFalse();

		var linkCount =
			conn.ExecuteScalar<long>(
				$"SELECT COUNT(*) FROM TagAliasLinks WHERE AliasTagId = {aliasTag.Id.Value} AND CanonicalTagId = {canonicalTag.Id.Value}");

		linkCount.Should().Be(1);
	}
}