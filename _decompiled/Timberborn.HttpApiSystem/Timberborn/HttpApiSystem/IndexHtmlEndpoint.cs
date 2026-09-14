using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Timberborn.SingletonSystem;

namespace Timberborn.HttpApiSystem;

internal class IndexHtmlEndpoint : IHttpApiEndpoint, ILoadableSingleton
{
	private static readonly string TemplatePath = Path.Combine(HttpApi.RootPath, "index.hbs");

	private readonly HttpApiCacheBuster _httpApiCacheBuster;

	private readonly ImmutableArray<IHttpApiPageSection> _httpApiPageSections;

	private HandlebarsTemplate<object, object> _template;

	private ImmutableArray<IHttpApiPageSection> _httpApiPageSectionsOrdered;

	internal IndexHtmlEndpoint(HttpApiCacheBuster httpApiCacheBuster, IEnumerable<IHttpApiPageSection> httpApiPageSections)
	{
		_httpApiCacheBuster = httpApiCacheBuster;
		_httpApiPageSections = httpApiPageSections.ToImmutableArray();
	}

	public void Load()
	{
		_template = Handlebars.Compile(File.ReadAllText(TemplatePath));
		_httpApiPageSectionsOrdered = _httpApiPageSections.OrderBy((IHttpApiPageSection section) => section.Order).ToImmutableArray();
	}

	public async Task<bool> TryHandle(HttpListenerContext context)
	{
		if (context.Request.Url.AbsolutePath == "/")
		{
			await Handle(context);
			return true;
		}
		return false;
	}

	private async Task Handle(HttpListenerContext context)
	{
		string text = _template(new
		{
			cacheBuster = _httpApiCacheBuster.CacheBuster,
			bodySections = _httpApiPageSectionsOrdered.Select((IHttpApiPageSection section) => section.BuildBody()).ToImmutableArray(),
			footerSections = (from section in _httpApiPageSectionsOrdered
				select section.BuildFooter() into footer
				select footer ?? "").ToImmutableArray()
		});
		await context.WriteHtml(text);
	}
}
