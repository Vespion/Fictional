using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.GraphTests;

public class DeleteTests
{
	[Fact]
	public Task DeletesLink_PropertyTest()
	{
		return TagData
			.TagBatchGen()
			.SampleAsync(DeletesLink);
	}

	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	private static async Task DeletesLink(SerializableTag[] tags)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var canonicalId = tags[0].Id;

		for (var i = 1; i < tags.Length; i++)
			conn.ExecuteNonQuery(
				$"INSERT INTO TagGraphLinks (ParentTagId, ChildTagId) VALUES ({canonicalId.Value}, {tags[i].Id.Value})");

		(await repo.RemoveGraphLinkAsync(canonicalId, tags[1].Id, TestContext.Current.CancellationToken)).Should()
			.BeTrue();

		var aliasIds =
			conn.Query<TagId>($"SELECT ChildTagId FROM TagGraphLinks WHERE ParentTagId = {canonicalId.Value}");

		aliasIds.Should().BeEquivalentTo(tags.Skip(2).Select(t => t.Id));
	}
}