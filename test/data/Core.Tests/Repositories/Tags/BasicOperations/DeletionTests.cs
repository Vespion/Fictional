using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.BasicOperations;

public class DeletionTests
{
	[Fact]
	public Task DeletesExistingObject_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(DeletesExistingObject);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task DeletesExistingObject(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		await repo.DeleteAsync(tag.Id, TestContext.Current.CancellationToken);

		TestHelpers.TagExists(conn, tag.Id).Should().BeFalse();
	}


	[Fact]
	public Task IgnoresMissingObject_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(IgnoresMissingObject);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task IgnoresMissingObject(SerializableTag tag)
	{
		var tgtId = new TagId(tag.Id.Value + 1);

		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		await repo.DeleteAsync(tgtId, TestContext.Current.CancellationToken);

		var result = await repo.ExistsAsync(tgtId, TestContext.Current.CancellationToken);

		result.Should().BeFalse();
	}
}