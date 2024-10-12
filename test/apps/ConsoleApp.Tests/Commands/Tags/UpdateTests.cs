using Fictional.App.ConsoleApp.Commands.Tags;
using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fictional.App.ConsoleApp.Tests.Commands.Tags;

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

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			newName,
			null,
			null,
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(newName);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().BeEquivalentTo(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesHiddenSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			null,
			null,
			null,
			!tag.Hidden,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().BeEquivalentTo(tag.Colour);
		result.Hidden.Should().Be(!tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesShorthandSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var newShorthand = string.IsNullOrWhiteSpace(tag.Shorthand) ? "TST" : string.Concat(tag.Shorthand.Reverse());

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			null,
			newShorthand,
			null,
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(newShorthand);
		result.Colour.Should().BeEquivalentTo(tag.Colour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesShorthandToNullSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			null,
			"",
			null,
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().BeNull();
		result.Colour.Should().BeEquivalentTo(tag.Colour);
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

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			null,
			null,
			newColour.ToString(),
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().BeEquivalentTo(newColour);
		result.Hidden.Should().Be(tag.Hidden);
	}

	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task UpdatesColourToNullSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var repo = new SqliteTagRepository(conn);

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			null,
			null,
			"",
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(tag.Name);
		result.Shorthand.Should().Be(tag.Shorthand);
		result.Colour.Should().BeNull();
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

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			newName,
			newShorthand,
			"",
			!tag.Hidden,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var resultTask = await repo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		resultTask.HasValue.Should().BeTrue();

		var result = resultTask!.Value;

		result.Id.Should().Be(tag.Id);
		result.Name.Should().Be(newName);
		result.Shorthand.Should().Be(newShorthand);
		result.Colour.Should().BeNull();
		result.Hidden.Should().Be(!tag.Hidden);
	}

	[Fact]
	public async Task ThrowsIfTagDoesNotExist()
	{
		var conn = await TestHelpers.ProvisionDatabase();

		var repo = new SqliteTagRepository(conn);

		var tag = new Tag((TagId)1, "Test", "tst", null, false);

		var resultCode = await UpdateTagCommand.Handle(
			NullLogger<UpdateTagCommand>.Instance,
			repo,
			tag.Id.Value,
			"newName",
			null,
			null,
			null,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(1);
	}
}