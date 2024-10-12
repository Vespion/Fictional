using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.BasicOperations;

public class ExistsTests
{
	[Fact]
	public Task ReturnsFalseForMissing_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(ReturnsFalseForMissing);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	private static async Task ReturnsFalseForMissing(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var result = await repo.ExistsAsync(new TagId(tag.Id + 1), TestContext.Current.CancellationToken);

		result.Should().BeFalse();
	}

	[Fact]
	public Task ReturnsTrueForExisting_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(ReturnsTrueForExisting);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	private static async Task ReturnsTrueForExisting(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var result = await repo.ExistsAsync(tag.Id, TestContext.Current.CancellationToken);

		result.Should().BeTrue();
	}
}