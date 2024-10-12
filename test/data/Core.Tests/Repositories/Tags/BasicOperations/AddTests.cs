using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.BasicOperations;

public class AddTests
{
	[Fact]
	public Task AddsTagToDatabase_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(AddsTagToDatabase);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	private static async Task AddsTagToDatabase(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		var repo = new SqliteTagRepository(conn);

		await repo.AddAsync(tag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().Be(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}
}