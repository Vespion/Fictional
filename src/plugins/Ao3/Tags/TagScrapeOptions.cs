namespace Fictional.Plugins.Ao3.Tags;

internal record TagScrapeOptions(
	Uri TagUrl,
	bool IgnoreRobotsConfiguration
);