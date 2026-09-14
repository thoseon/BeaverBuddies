using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Timberborn.HttpApiSystem;

internal class HttpAdaptersJsonEndpoint : IHttpApiEndpoint
{
	private readonly HttpApiIntermediary _httpApiIntermediary;

	private readonly Regex _adaptersPath = new Regex("^/api/adapters/?$", RegexOptions.Compiled);

	private readonly Regex _adapterPath = new Regex("^/api/adapters/(?<name>[^/]+)/?$", RegexOptions.Compiled);

	public HttpAdaptersJsonEndpoint(HttpApiIntermediary httpApiIntermediary)
	{
		_httpApiIntermediary = httpApiIntermediary;
	}

	public async Task<bool> TryHandle(HttpListenerContext context)
	{
		string absolutePath = context.Request.Url.AbsolutePath;
		Match match = _adaptersPath.Match(absolutePath);
		if (match != null && match.Success)
		{
			await ProcessAdapters(context);
			return true;
		}
		Match match2 = _adapterPath.Match(absolutePath);
		if (match2 != null && match2.Success)
		{
			await ProcessAdapter(context, match2);
			return true;
		}
		return false;
	}

	private async Task ProcessAdapters(HttpListenerContext context)
	{
		await context.WriteJson(_httpApiIntermediary.GetAdapters().Select(Json));
	}

	private async Task ProcessAdapter(HttpListenerContext context, Match match)
	{
		string text = Uri.UnescapeDataString(match.Groups["name"].Value);
		if (_httpApiIntermediary.TryGetAdapter(text, out var httpAdapterSnapshot))
		{
			await context.WriteJson(Json(httpAdapterSnapshot));
		}
		else
		{
			await context.WriteText("HttpAdapter not found: " + text, 404);
		}
	}

	private static object Json(HttpAdapterSnapshot adapter)
	{
		return new
		{
			name = adapter.Name,
			state = adapter.State
		};
	}
}
