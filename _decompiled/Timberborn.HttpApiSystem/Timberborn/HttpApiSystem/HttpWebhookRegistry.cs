using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.EntitySystem;

namespace Timberborn.HttpApiSystem;

public class HttpWebhookRegistry
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	public HttpWebhookRegistry(EntityComponentRegistry entityComponentRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
	}

	public ImmutableArray<string> FindUnsafeAddresses()
	{
		return (from address in (from url in _entityComponentRegistry.GetAll<HttpAdapter>().SelectMany((HttpAdapter client) => client.AllWebhookUrls)
				where !string.IsNullOrWhiteSpace(url)
				where !UrlIsSafe(url)
				select url).Select(GetHostIfPossible).Distinct()
			orderby address
			select address).ToImmutableArray();
	}

	private static bool UrlIsSafe(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			return result.IsLoopback;
		}
		return false;
	}

	private static string GetHostIfPossible(string url)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			return url;
		}
		return result.Host;
	}
}
