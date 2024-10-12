using Fictional.App.ConsoleApp.Commands.Tags.Alias;
using Fictional.Data.Core.Repositories.Sql;
using Fictional.TestUtils;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fictional.App.ConsoleApp.Tests.Commands.Tags.Alias;

public class DeleteTests
{
	[Theory]
	[MemberData(nameof(TagData.TagPair), MemberType = typeof(TagData))]
	public async Task RemovesAliasFromTag(SerializableTag canonicalTag, SerializableTag aliasTag)
	{
		var conn = await TestHelpers.ProvisionDatabase();

		TestHelpers.AddTags(conn, [canonicalTag, aliasTag]);

		var repo = new SqliteTagRepository(conn);

		await repo.AddAliasLinkAsync(canonicalTag.Id, aliasTag.Id, TestContext.Current.CancellationToken);

		var result = await DeleteTagAliasCommand.Handle(
			NullLogger<DeleteTagAliasCommand>.Instance,
			repo,
			canonicalTag.Id.Value,
			aliasTag.Id.Value,
			TestContext.Current.CancellationToken
		);

		result.Should().Be(0);

		var aliases = await repo.GetAliasesAsync(canonicalTag.Id, TestContext.Current.CancellationToken);

		aliases.Should().HaveCount(0);
	}
}