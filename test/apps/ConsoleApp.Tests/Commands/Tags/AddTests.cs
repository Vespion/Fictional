using Fictional.App.ConsoleApp.Commands.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fictional.App.ConsoleApp.Tests.Commands.Tags;

public class AddTests
{
	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task AddSimpleTagSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		var tagRepo = new SqliteTagRepository(conn);

		var resultCode = await NewTagCommand.Handle(
			NullLogger<NewTagCommand>.Instance,
			tagRepo,
			tag.Name,
			tag.Shorthand,
			tag.Colour,
			tag.Hidden,
			[],
			[],
			tag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeTrue();
		createdTag!.Value.Should().BeEquivalentTo(tag);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task AddTagWithAliasSuccessfully(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, aliasTag);

		var tagRepo = new SqliteTagRepository(conn);

		var resultCode = await NewTagCommand.Handle(
			NullLogger<NewTagCommand>.Instance,
			tagRepo,
			canonicalTag.Name,
			canonicalTag.Shorthand,
			canonicalTag.Colour,
			canonicalTag.Hidden,
			[],
			[aliasTag.Id.Value],
			canonicalTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeTrue();
		createdTag!.Value.Should().BeEquivalentTo(canonicalTag);

		var aliases = await tagRepo.GetAliasesAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(1);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task AddTagWithParentSuccessfully(SerializableTag parentTag, SerializableTag childTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, childTag);

		var tagRepo = new SqliteTagRepository(conn);

		var resultCode = await NewTagCommand.Handle(
			NullLogger<NewTagCommand>.Instance,
			tagRepo,
			parentTag.Name,
			parentTag.Shorthand,
			parentTag.Colour,
			parentTag.Hidden,
			[childTag.Id.Value],
			[],
			parentTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(parentTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeTrue();
		createdTag!.Value.Should().BeEquivalentTo(parentTag);

		var aliases = await tagRepo.GetChildrenAsync(parentTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(1);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task AddTagWithParentAndAliasSuccessfully(SerializableTag parentTag, SerializableTag childTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, childTag);

		var tagRepo = new SqliteTagRepository(conn);

		var resultCode = await NewTagCommand.Handle(
			NullLogger<NewTagCommand>.Instance,
			tagRepo,
			parentTag.Name,
			parentTag.Shorthand,
			parentTag.Colour,
			parentTag.Hidden,
			[childTag.Id.Value],
			[childTag.Id.Value],
			parentTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(parentTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeTrue();
		createdTag!.Value.Should().BeEquivalentTo(parentTag);

		var aliases = await tagRepo.GetAliasesAsync(parentTag.Id, TestContext.Current.CancellationToken);
		var children = await tagRepo.GetChildrenAsync(parentTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(1);
		children.Should().HaveCount(1);
	}
}