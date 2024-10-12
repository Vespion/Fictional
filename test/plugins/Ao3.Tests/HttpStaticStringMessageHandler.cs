using System.Net;

namespace Fictional.Plugins.Ao3.Tests;

public class HttpStaticStringMessageHandler(IDictionary<string, string> pages): HttpMessageHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		HttpResponseMessage response;
		if (!pages.TryGetValue(
			    request.RequestUri?.AbsoluteUri ?? string.Empty,
			    out var html)
		   )
		{
			if (!pages.TryGetValue("*", out var fallbackHtml))
			{
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
			}

			response = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(fallbackHtml)
			};

		}
		else
		{
			response = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(html)
			};
		}

		return Task.FromResult(response);

	}
}