using System.Runtime.CompilerServices;

namespace Fictional.Plugins.PluginRuntimeLib.Scraping;

/// <summary>
/// Helper methods for web scraping.
/// </summary>
public static class ScraperExtensions
{
	/// <summary>
	/// Gets the authority of the URI, as a URI.
	/// </summary>
	/// <param name="uri">The target URI</param>
	/// <returns>The scheme and authority components as a new URI</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Uri GetAuthority(this Uri uri) => new UriBuilder(uri.Scheme, uri.Authority).Uri;
}