using Fictional.Data.Core.Models.Tags;

namespace Fictional.Plugins.PluginRuntimeLib.Scraping.Tags;

/// <summary>
/// Represents the results of a tag scrape operation.
/// </summary>
/// <param name="Root">
/// The root tag that was scraped.
/// </param>
/// <param name="AliasTagResults">
/// The results of scraping any alias tags.
/// </param>
/// <param name="ParentTagResults">
/// The results of scraping any parent tags.
/// </param>
/// <param name="ChildTagResults">
/// The results of scraping any child tags.
/// </param>
public record TagScrapeResults(Tag Root, ICollection<TagScrapeResults> AliasTagResults, ICollection<TagScrapeResults> ParentTagResults, ICollection<TagScrapeResults> ChildTagResults);