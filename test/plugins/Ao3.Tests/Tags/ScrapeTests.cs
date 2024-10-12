using Fictional.Plugins.Ao3.Tags;
using Fictional.Plugins.Ao3.Tests.Data.Tags;
using Fictional.TestUtils.Logging;
using Microsoft.Extensions.Logging;

namespace Fictional.Plugins.Ao3.Tests.Tags;

public class ScrapeTests
{
	[Theory]
	[MemberData(nameof(TagPageData.AllTagPages), MemberType = typeof(TagPageData))]
	internal async Task ScrapesTagSuccessfully(string html, TagCategory category, bool canonical, string expectedTagName, int expectedAliasCount, int expectedParentCount, int expectedMetaCount, int expectedSubCount)
	{
		var logging = new LoggerFactory([new XunitLoggingProvider()]);
		var httpClient = new HttpClient(new HttpStaticStringMessageHandler(new Dictionary<string, string>
		{
			{ $"https://example.com/tag/{expectedTagName}", html },
			{ "*", "<html/>"}
		}));
		var scraper = new TagScraper(httpClient, logging.CreateLogger<TagScraper>());

		var tag = await scraper.Scrape(new Uri($"https://example.com/tag/{expectedTagName}"), true, TestContext.Current.CancellationToken);

		var parentCount = expectedParentCount + expectedMetaCount + 1;
		if (canonical)
		{
			parentCount++;
		}

		tag.Root.Name.Should().Be(expectedTagName);
		tag.AliasTagResults.Should().HaveCount(expectedAliasCount);
		tag.ParentTagResults.Should().HaveCount(parentCount);
		tag.ChildTagResults.Should().HaveCount(expectedSubCount);

		tag.ParentTagResults.Should().Contain(results => results.Root.Name == category.ToString());
		if (canonical)
		{
			tag.ParentTagResults.Should().Contain(results => results.Root.Name == "Canonical");
		}
		else
		{
			tag.ParentTagResults.Should().NotContain(results => results.Root.Name == "Canonical");
		}
	}
}