using System.Diagnostics;
using System.Xml.XPath;
using Fictional.Data.Core.Models.Tags;
using Fictional.Plugins.PluginRuntimeLib.Scraping.Tags;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Fictional.Plugins.Ao3.Tags;

internal class TagScraper(HttpClient httpClient, ILogger<TagScraper> logger) : TagScraperBase<TagScraper>(httpClient, logger)
{
	private static XPathExpression? _aliasExpression;
	private static XPathExpression? _childExpression;
	private static XPathExpression? _parentExpression;
	private static XPathExpression? _metaExpression;
	private static XPathExpression? _tagNameExpression;

	private static XPathExpression AliasExpression => _aliasExpression ??= XPathExpression.Compile("//*[@id=\"main\"]/div[2]/div[3]/ul/li/a");
	private static XPathExpression ChildExpression => _childExpression ??= XPathExpression.Compile("//*[@id=\"main\"]/div[2]/div[5]/ul/li/a");
	private static XPathExpression ParentExpression => _parentExpression ??= XPathExpression.Compile("//*[@id=\"main\"]/div[2]/div[2]/ul/li/a");
	private static XPathExpression MetaExpression => _metaExpression ??= XPathExpression.Compile("//*[@id=\"main\"]/div[2]/div[4]/ul/li/a");
	private static XPathExpression TagNameExpression => _tagNameExpression ??= XPathExpression.Compile("//*[@id=\"main\"]/div[2]/div[1]/h2");


	protected override IEnumerable<Uri> ExtractChildLinks(HtmlNode arg)
	{
		var listNode = arg.SelectNodes(ChildExpression);

		foreach (var child in listNode)
		{
			var childRef = child.GetAttributeValue("href", null);

			if (childRef != null)
			{
				yield return new Uri(childRef, UriKind.Relative);
			}
		}
	}

	protected override IEnumerable<Uri> ExtractParentLinks(HtmlNode arg)
	{
		var parentListNode = arg.SelectNodes(ParentExpression);

		foreach (var child in parentListNode)
		{
			var childRef = child.GetAttributeValue("href", null);

			if (childRef != null)
			{
				yield return new Uri(childRef, UriKind.Relative);
			}
		}

		var metaListNode = arg.SelectNodes(MetaExpression);

		foreach (var child in metaListNode)
		{
			var childRef = child.GetAttributeValue("href", null);

			if (childRef != null)
			{
				yield return new Uri(childRef, UriKind.Relative);
			}
		}
	}

	protected override IEnumerable<Uri> ExtractAliasLinks(HtmlNode arg)
	{
		var listNode = arg.SelectNodes(AliasExpression);

		foreach (var child in listNode)
		{
			var childRef = child.GetAttributeValue("href", null);

			if (childRef != null)
			{
				yield return new Uri(childRef, UriKind.Relative);
			}
		}
	}

	protected override Tag ExtractTagData(HtmlNode document)
	{
		var headerNode = document.SelectSingleNode(TagNameExpression);
		var text = headerNode.InnerText;

		Debug.Assert(text is not null, "text is null");

		return new Tag(default, text, default, default, default);
	}
}