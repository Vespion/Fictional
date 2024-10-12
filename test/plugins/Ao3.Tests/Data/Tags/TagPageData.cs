using Fictional.Plugins.Ao3.Tags;

namespace Fictional.Plugins.Ao3.Tests.Data.Tags;

internal record TagPageRecord(string Html, TagCategory Category, bool Canonical, string ExpectedTagName, int ExpectedAliasCount, int ExpectedParentCount, int ExpectedMetaCount, int ExpectedSubCount);

internal static class TagPageData
{
	public static IEnumerable<TheoryDataRow<string, TagCategory, bool, string, int, int, int, int>> AllTagPages()
	{
		yield return Victorian();
	}

	public static TheoryDataRow<string, TagCategory, bool, string, int, int, int, int> Victorian()
	{
		using var reader = EmbeddedResources.Data_Tags_Pages_Victorian___Archive_of_Our_Own_html_Reader;
		var html = reader.ReadToEnd();

		return (html, TagCategory.Additional, true, "Victorian", 190, 1, 1, 3);
	}
}