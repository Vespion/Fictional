using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.BasicOperations;

public class FetchTests
{
	[Fact]
	// ReSharper disable once UnusedMember.Local
	private static async Task ReturnsNullForMissing()
	{
		var id = new TagId(1);

		var conn = await TestHelpers.ProvisionDatabase();

		var repo = new SqliteTagRepository(conn);

		var result = await repo.GetByIdAsync(id, TestContext.Current.CancellationToken);

		result.Should().BeNull();
	}

	[Fact]
	public Task ReturnsObject_PropertyTest()
	{
		return TagData.TagGen().SampleAsync(ReturnsObject);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	private static async Task ReturnsObject(SerializableTag tag)
	{
		// var fileConn = new SqliteConnection(@"Data Source=I:\Programming\Fictional\dev.sqlite");
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var result = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);
		result.HasValue.Should().BeTrue();
		result!.Value.Id.Should().Be(tag.Id);
		result.Value.Name.Should().Be(tag.Name);
		result.Value.Shorthand.Should().Be(tag.Shorthand);
		result.Value.Colour.Should().Be(tag.Colour);

		result.Value.Hidden.Should().Be(tag.Hidden);
	}

	[Fact]
	public Task ReturnsAllObjects_PropertyTest()
	{
		return TagData.TagGen()
			.Select(TagData.TagGen(), TagData.TagGen(), TagData.TagGen())
			.Where((tag1, tag2, tag3, tag4) =>
			{
				var ids = new HashSet<TagId> { tag1.Id, tag2.Id, tag3.Id, tag4.Id };
				return ids.Count == 4;
			})
			.Select((tag1, tag2, tag3, tag4) => new[] { tag1, tag2, tag3, tag4 })
			.SampleAsync(ReturnsAllObjects);
	}

	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	private async Task ReturnsAllObjects(SerializableTag[] tags)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var result = repo
			.GetAllAsync(TestContext.Current.CancellationToken)
			.ToBlockingEnumerable(TestContext.Current.CancellationToken)
			.ToArray();

		result.Should().BeEquivalentTo(tags);
	}

	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	public async Task GetAllCanBeCancelled(SerializableTag[] tags)
	{
		var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, tags);

		var repo = new SqliteTagRepository(conn);

		var result = repo
			.GetAllAsync(tokenSource.Token);

		var resultList = new List<Tag>(3);

		var count = 0;
		await foreach (var t in result)
			if (count == 3)
			{
				await tokenSource.CancelAsync();
			}
			else
			{
				resultList.Add(t);
				count++;
			}

		tokenSource.Token.IsCancellationRequested.Should().BeTrue();
		count.Should().Be(3);
		resultList.Count.Should().Be(3);
	}
}