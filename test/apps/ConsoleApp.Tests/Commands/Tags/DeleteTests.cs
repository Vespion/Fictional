using Fictional.App.ConsoleApp.Commands.Tags;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fictional.App.ConsoleApp.Tests.Commands.Tags;

public class DeleteTests
{
	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public async Task DeletesSimpleTagSuccessfully(SerializableTag tag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTag(conn, tag);

		var tagRepo = new SqliteTagRepository(conn);

		var resultCode = await DeleteTagCommand.Handle(
			NullLogger<DeleteTagCommand>.Instance,
			tagRepo,
			tag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(tag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeFalse();
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task DeleteTagWithAliasSuccessfully(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var tagRepo = new SqliteTagRepository(conn);

		await tagRepo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken);

		var resultCode = await DeleteTagCommand.Handle(
			NullLogger<DeleteTagCommand>.Instance,
			tagRepo,
			canonicalTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeFalse();

		var alias = await tagRepo.GetByIdAsync(aliasTag.Id, TestContext.Current.CancellationToken);

		alias.HasValue.Should().BeTrue();

		conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM TagAliasLinks WHERE AliasTagId = {aliasTag.Id.Value}")
			.Should().Be(0);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task DeleteAliasTagSuccessfully(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var tagRepo = new SqliteTagRepository(conn);

		await tagRepo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken);

		var resultCode = await DeleteTagCommand.Handle(
			NullLogger<DeleteTagCommand>.Instance,
			tagRepo,
			aliasTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(aliasTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeFalse();

		var canonical = await tagRepo.GetByIdAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		canonical.HasValue.Should().BeTrue();

		conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM TagAliasLinks WHERE CanonicalTagId = {canonicalTag.Id.Value}")
			.Should().Be(0);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task DeleteTagWithChildSuccessfully(SerializableTag parentTag, SerializableTag childTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [parentTag, childTag]);

		var tagRepo = new SqliteTagRepository(conn);

		await tagRepo.AddGraphLinkAsync(parentTag.Id, childTag.Id, TestContext.Current.CancellationToken);

		var resultCode = await DeleteTagCommand.Handle(
			NullLogger<DeleteTagCommand>.Instance,
			tagRepo,
			parentTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(parentTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeFalse();

		var alias = await tagRepo.GetByIdAsync(childTag.Id, TestContext.Current.CancellationToken);

		alias.HasValue.Should().BeTrue();

		conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM TagGraphLinks WHERE ChildTagId = {childTag.Id.Value}")
			.Should().Be(0);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task DeleteTagWithParentSuccessfully(SerializableTag parentTag, SerializableTag childTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [parentTag, childTag]);

		var tagRepo = new SqliteTagRepository(conn);

		await tagRepo.AddGraphLinkAsync(parentTag.Id, childTag.Id, TestContext.Current.CancellationToken);

		var resultCode = await DeleteTagCommand.Handle(
			NullLogger<DeleteTagCommand>.Instance,
			tagRepo,
			childTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		resultCode.Should().Be(0);

		var createdTag = await tagRepo.GetByIdAsync(childTag.Id, TestContext.Current.CancellationToken);

		createdTag.HasValue.Should().BeFalse();

		var canonical = await tagRepo.GetByIdAsync(parentTag.Id, TestContext.Current.CancellationToken);

		canonical.HasValue.Should().BeTrue();

		conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM TagGraphLinks WHERE ParentTagId = {parentTag.Id.Value}")
			.Should().Be(0);
	}
}