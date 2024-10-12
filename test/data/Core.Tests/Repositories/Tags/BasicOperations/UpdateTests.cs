using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Repositories.Tags.BasicOperations;

public class UpdateTests
{
	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesNameSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newName = string.Concat(tag.Name.Reverse());

		var newTag = tag with { Name = newName };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(newTag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().Be(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesHiddenSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newTag = tag with { Hidden = !tag.Hidden };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(newTag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().Be(tag.Colour);
		result.Hidden.Should().Be(newTag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesShorthandSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newShorthand = string.IsNullOrWhiteSpace(tag.Shorthand) ? "TST" : string.Concat(tag.Shorthand.Reverse());

		var newTag = tag with { Shorthand = newShorthand };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(newTag.Shorthand);
		result.Colour.Should().Be(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesShorthandToNullSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newTag = tag with { Shorthand = null };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(newTag.Shorthand);
		result.Colour.Should().Be(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesColourSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newColour = tag.Colour == null ? new Colour(50, 150, 250) : tag.Colour.Value with { R = tag.Colour.Value.B, B = tag.Colour.Value.R };

		var newTag = tag with { Colour = newColour };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().Be(newTag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesColourToNullSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newTag = tag with { Colour = null };

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().Be(newTag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesMultipleElementsSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newName = string.Concat(tag.Name.Reverse());
		var newShorthand = string.IsNullOrWhiteSpace(tag.Shorthand) ? "TST" : string.Concat(tag.Shorthand.Reverse());

		var newTag = tag with { Name = newName, Shorthand = newShorthand, Hidden = !tag.Hidden, Colour = null};

		await repo.UpdateAsync(newTag, TestContext.Current.CancellationToken);

		var result = conn.QuerySingle<Tag>($"SELECT * FROM Tags WHERE Id = {tag.Id.Value}");

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(newTag.Name);
		result.Shorthand.Should().Be(newTag.Shorthand);
		result.Colour.Should().Be(newTag.Colour);
		result.Hidden.Should().Be(newTag.Hidden);
	}

	[Fact]
	public async Task ThrowsIfTagDoesNotExist()
	{
		var conn = await TestHelpers.ProvisionDatabase();

		var repo = new SqliteTagRepository(conn);

		var tag = new Tag((TagId)1, "Test", "tst", null, false);

		await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.UpdateAsync(tag, TestContext.Current.CancellationToken).AsTask());
	}
}