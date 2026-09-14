using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using HandlebarsDotNet;
using Timberborn.SingletonSystem;

namespace Timberborn.HttpApiSystem;

internal class HttpAdapterPageSection : IHttpApiPageSection, ILoadableSingleton
{
	private static readonly string TemplatePath = Path.Combine(HttpApi.RootPath, "index-adapters.hbs");

	private readonly HttpApiIntermediary _httpApiIntermediary;

	private HandlebarsTemplate<object, object> _template;

	public int Order => 200;

	internal HttpAdapterPageSection(HttpApiIntermediary httpApiIntermediary)
	{
		_httpApiIntermediary = httpApiIntermediary;
	}

	public void Load()
	{
		_template = Handlebars.Compile(File.ReadAllText(TemplatePath));
	}

	public string BuildBody()
	{
		ImmutableArray<HttpAdapterSnapshot> adapters = _httpApiIntermediary.GetAdapters();
		return _template(new
		{
			adaptersUrl = "/api/adapters",
			adapters = from adapter in adapters
				orderby adapter.Name
				select new
				{
					name = adapter.Name,
					state = adapter.State,
					url = "/api/adapters/" + Uri.EscapeDataString(adapter.Name)
				}
		});
	}

	public string BuildFooter()
	{
		return "";
	}
}
