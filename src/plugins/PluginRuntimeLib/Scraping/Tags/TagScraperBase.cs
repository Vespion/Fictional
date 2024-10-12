using System.Net;
using System.Threading.Tasks.Dataflow;
using Fictional.Data.Core.Models.Tags;
using Fictional.Plugins.PluginRuntimeLib.Exceptions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using TurnerSoftware.RobotsExclusionTools;

namespace Fictional.Plugins.PluginRuntimeLib.Scraping.Tags;

/// <summary>
/// Base class for tag scrapers.
/// </summary>
public abstract class TagScraperBase<TLogger>(HttpClient httpClient, ILogger<TLogger> logger) where TLogger : TagScraperBase<TLogger>
{
	/// <summary>
	/// The HTTP client to use for downloading pages.
	/// </summary>
	protected readonly HttpClient HttpClient = httpClient;

	public async ValueTask<TagScrapeResults> Scrape(Uri targetUrl, bool ignoreRobotsConfiguration, CancellationToken cancellationToken = default)
	{
		var authority = targetUrl.GetAuthority();

		var doc = await Download(targetUrl, ignoreRobotsConfiguration);

		var aliasLinks = ExtractAliasLinks(doc.DocumentNode).ToArray();
		var childLinks = ExtractChildLinks(doc.DocumentNode).ToArray();
		var parentLinks = ExtractParentLinks(doc.DocumentNode).ToArray();

		var rootTask = ExtractTagData(doc.DocumentNode);


		var aliasResults = new List<TagScrapeResults>();
		var childResults = new List<TagScrapeResults>();
		var parentResults = new List<TagScrapeResults>();

		var aliasTask = Parallel.ForEachAsync(aliasLinks, cancellationToken, async (uri, token) =>
		{
			if (!uri.IsAbsoluteUri)
			{
				uri = new Uri(authority, uri);
			}

			try
			{
				aliasResults.Add(await Scrape(uri, ignoreRobotsConfiguration, token));
			}
			catch (HttpRequestException ex)
			{
				if(ex.StatusCode == HttpStatusCode.NotFound)
				{
					logger.LogWarning("Alias not found: {Alias}, it will not be included in the tag tree", uri);
				}
				else
				{
					logger.LogError(ex, "HTTP error scraping alias: {Alias}", uri);
				}
			}
		});

		var childTask = Parallel.ForEachAsync(childLinks, cancellationToken, async (uri, token) =>
		{
			if (!uri.IsAbsoluteUri)
			{
				uri = new Uri(authority, uri);
			}

			try
			{
				childResults.Add(await Scrape(uri, ignoreRobotsConfiguration, token));
			}
			catch (HttpRequestException ex)
			{
				if(ex.StatusCode == HttpStatusCode.NotFound)
				{
					logger.LogWarning("Child not found: {Child}, it will not be included in the tag tree", uri);
				}
				else
				{
					logger.LogError(ex, "HTTP error scraping child: {Child}", uri);
				}
			}
		});

		var parentTask = Parallel.ForEachAsync(parentLinks, cancellationToken, async (uri, token) =>
		{
			if (!uri.IsAbsoluteUri)
			{
				uri = new Uri(authority, uri);
			}

			try
			{
				parentResults.Add(await Scrape(uri, ignoreRobotsConfiguration, token));
			}
			catch (HttpRequestException ex)
			{
				if(ex.StatusCode == HttpStatusCode.NotFound)
				{
					logger.LogWarning("Parent not found: {Parent}, it will not be included in the tag tree", uri);
				}
				else
				{
					logger.LogError(ex, "HTTP error scraping parent: {Parent}", uri);
				}
			}
		});

		await Task.WhenAll(aliasTask, childTask, parentTask);

		return new TagScrapeResults(rootTask, aliasResults, parentResults, childResults);
	}

	/// <summary>
	/// Extract child links from the HTML document.
	/// </summary>
	/// <param name="arg">The document to scrape</param>
	/// <returns>Additional <see cref="Uri"/> to scrape</returns>
	protected abstract IEnumerable<Uri> ExtractChildLinks(HtmlNode arg);

	/// <summary>
	/// Extract parent links from the HTML document.
	/// </summary>
	/// <param name="arg">The document to scrape</param>
	/// <returns>Additional <see cref="Uri"/> to scrape</returns>
	protected abstract IEnumerable<Uri> ExtractParentLinks(HtmlNode arg);

	/// <summary>
	/// Extract alias links from the HTML document.
	/// </summary>
	/// <param name="arg">The document to scrape</param>
	/// <returns>Additional <see cref="Uri"/> to scrape</returns>
	protected abstract IEnumerable<Uri> ExtractAliasLinks(HtmlNode arg);

	/// <summary>
	/// Downloads the target URL and parses the HTML document.
	/// </summary>
	/// <param name="targetUrl">The target URL to scrape</param>
	/// <param name="ignoreRobotsConfiguration">Whether to ignore robots.txt and X-Robots-Tag headers</param>
	/// <returns>The parsed HTML document</returns>
	/// <exception cref="AccessDeniedByOrigin">Thrown when access to the target URL is denied by the origin's robots.txt file or X-Robots-Tag header</exception>
	protected virtual async Task<HtmlDocument> Download(Uri targetUrl, bool ignoreRobotsConfiguration)
	{
		var authority = targetUrl.GetAuthority();
		if (!ignoreRobotsConfiguration)
		{
			await using var robotsStream = await HttpClient.GetStreamAsync(new Uri(authority, "robots.txt"));
			var robots = new RobotsFileParser();
			var robotsFile = await robots.FromStreamAsync(robotsStream, authority);

			if (!robotsFile.IsAllowedAccess(targetUrl, HttpClient.DefaultRequestHeaders.UserAgent.ToString()))
			{
				throw new AccessDeniedByOrigin("Access to the target URL is denied by the origin's robots.txt file.", authority.ToString(), true);
			}
		}


		using var response = await HttpClient.GetAsync(targetUrl);

		response.EnsureSuccessStatusCode();

		var pageRules = ignoreRobotsConfiguration ? null : new List<string>();

		if (!ignoreRobotsConfiguration && response.Headers.TryGetValues("X-Robots-Tag", out var xRobotsTagValues))
		{
			pageRules!.AddRange(xRobotsTagValues);
		}

		await using var stream = await response.Content.ReadAsStreamAsync();

		var doc = new HtmlDocument();
		doc.Load(stream, true);


		if (!ignoreRobotsConfiguration)
		{
			var robots = new RobotsPageParser();
			var robotRules = robots.FromRules(pageRules);

			if(!robotRules.CanIndex(HttpClient.DefaultRequestHeaders.UserAgent.ToString()) ||
			   !robotRules.CanFollowLinks(HttpClient.DefaultRequestHeaders.UserAgent.ToString()))
			{
				throw new AccessDeniedByOrigin("Access to the target URL is denied by the origin's X-Robots-Tag header or Robots meta tag.", targetUrl.ToString(), true);
			}
		}

		return doc;
	}

	/// <summary>
	/// Extracts the tag data from the HTML document.
	/// </summary>
	/// <param name="document">The document to scrape</param>
	/// <returns>The extracted tag data</returns>
	protected abstract Tag ExtractTagData(HtmlNode document);
}