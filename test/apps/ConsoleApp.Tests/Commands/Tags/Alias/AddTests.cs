using Fictional.App.ConsoleApp.Commands.Tags.Alias;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fictional.App.ConsoleApp.Tests.Commands.Tags.Alias;

public class AddTests
{
	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task AddsAliasToTag(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var repo = new SqliteTagRepository(conn);

		var result = await AddTagAliasCommand.Handle(
			NullLogger<AddTagAliasCommand>.Instance,
			repo,
			canonicalTag.Id.Value,
			aliasTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		result.Should().Be(0);

		var aliases = await repo.GetAliasesAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(1);
	}

	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task WarnsAddingDuplicateAliasToTag(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var repo = new SqliteTagRepository(conn);

		(await repo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken)).Should()
			.BeTrue();

		var result = await AddTagAliasCommand.Handle(
			NullLogger<AddTagAliasCommand>.Instance,
			repo,
			canonicalTag.Id.Value,
			aliasTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		result.Should().Be(1);

		var aliases = await repo.GetAliasesAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(1);
	}
}