using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.AliasTests;

public class DeleteTests
{
	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	public async Task DeletesSimpleAliasesTag(SerializableTag[] tags)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var canonicalId = tags[0].Id;

		for (var i = 1; i < tags.Length; i++)
			conn.ExecuteNonQuery(
				$"INSERT INTO TagAliasLinks (CanonicalTagId, AliasTagId) VALUES ({canonicalId.Value}, {tags[i].Id.Value})");

		(await repo.RemoveAliasLinkAsync(canonicalId, tags[1].Id, TestContext.Current.CancellationToken)).Should()
			.BeTrue();

		var aliasIds =
			conn.Query<TagId>($"SELECT AliasTagId FROM TagAliasLinks WHERE CanonicalTagId = {canonicalId.Value}");

		aliasIds.Should().BeEquivalentTo(tags.Skip(2).Select(t => t.Id));
	}
}