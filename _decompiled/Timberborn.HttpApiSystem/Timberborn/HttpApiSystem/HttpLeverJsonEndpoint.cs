using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal class HttpLeverJsonEndpoint : IHttpApiEndpoint
{
	private readonly HttpApiIntermediary _httpApiIntermediary;

	private readonly Regex _leversPath = new Regex("^/api/levers/?$", RegexOptions.Compiled);

	private readonly Regex _leverPath = new Regex("^/api/levers/(?<name>[^/]+)/?$", RegexOptions.Compiled);

	private readonly Regex _switchOnPath = new Regex("^/api/switch-on/(?<name>[^/]+)/?$", RegexOptions.Compiled);

	private readonly Regex _switchOffPath = new Regex("^/api/switch-off/(?<name>[^/]+)/?$", RegexOptions.Compiled);

	private readonly Regex _colorPath = new Regex("^/api/color/(?<name>[^/]+)/(?<color>[0-9a-fA-F]{6})$/?$", RegexOptions.Compiled);

	public HttpLeverJsonEndpoint(HttpApiIntermediary httpApiIntermediary)
	{
		_httpApiIntermediary = httpApiIntermediary;
	}

	public async Task<bool> TryHandle(HttpListenerContext context)
	{
		string absolutePath = context.Request.Url.AbsolutePath;
		Match match = _leversPath.Match(absolutePath);
		if (match != null && match.Success)
		{
			await ProcessLevers(context);
			return true;
		}
		Match match2 = _leverPath.Match(absolutePath);
		if (match2 != null && match2.Success)
		{
			await ProcessLever(context, match2);
			return true;
		}
		Match match3 = _switchOnPath.Match(absolutePath);
		if (match3 != null && match3.Success)
		{
			await ProcessSwitch(context, match3, state: true);
			return true;
		}
		Match match4 = _switchOffPath.Match(absolutePath);
		if (match4 != null && match4.Success)
		{
			await ProcessSwitch(context, match4, state: false);
			return true;
		}
		Match match5 = _colorPath.Match(absolutePath);
		if (match5 != null && match5.Success)
		{
			await ProcessColor(context, match5);
			return true;
		}
		return false;
	}

	private async Task ProcessLevers(HttpListenerContext context)
	{
		await context.WriteJson(_httpApiIntermediary.GetLevers().Select(Json));
	}

	private async Task ProcessLever(HttpListenerContext context, Match match)
	{
		string name = Uri.UnescapeDataString(match.Groups["name"].Value);
		if (_httpApiIntermediary.TryGetLever(name, out var httpLeverSnapshot))
		{
			await context.WriteJson(Json(httpLeverSnapshot));
		}
		else
		{
			await Write404(context, name);
		}
	}

	private async Task ProcessSwitch(HttpListenerContext context, Match match, bool state)
	{
		string name = Uri.UnescapeDataString(match.Groups["name"].Value);
		if (_httpApiIntermediary.TryGetLever(name, out var _))
		{
			_httpApiIntermediary.AddLeverCommand(new HttpLeverCommand(name, state));
			await WriteOK(context);
		}
		else
		{
			await Write404(context, name);
		}
	}

	private async Task ProcessColor(HttpListenerContext context, Match match)
	{
		string name = Uri.UnescapeDataString(match.Groups["name"].Value);
		string text = Uri.UnescapeDataString(match.Groups["color"].Value);
		if (ColorUtility.TryParseHtmlString("#" + text, out var color))
		{
			if (_httpApiIntermediary.TryGetLever(name, out var _))
			{
				_httpApiIntermediary.AddLeverCommand(new HttpLeverCommand(name, color));
				await WriteOK(context);
			}
			else
			{
				await Write404(context, name);
			}
		}
		else
		{
			await context.WriteText("Invalid color", 400);
		}
	}

	private static async Task WriteOK(HttpListenerContext context)
	{
		await context.WriteText("OK", 200);
	}

	private static async Task Write404(HttpListenerContext context, string name)
	{
		await context.WriteText("HttpLever not found: " + name, 404);
	}

	private static object Json(HttpLeverSnapshot httpLeverSnapshot)
	{
		return new
		{
			name = httpLeverSnapshot.Name,
			state = httpLeverSnapshot.State,
			springReturn = httpLeverSnapshot.IsSpringReturn
		};
	}
}
