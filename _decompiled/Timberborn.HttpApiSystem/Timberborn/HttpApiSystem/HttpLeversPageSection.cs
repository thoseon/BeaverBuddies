using System;
using System.IO;
using System.Linq;
using HandlebarsDotNet;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal class HttpLeversPageSection : IHttpApiPageSection, ILoadableSingleton
{
	private static readonly string BodyTemplatePath = Path.Combine(HttpApi.RootPath, "index-levers.hbs");

	private static readonly string FooterTemplatePath = Path.Combine(HttpApi.RootPath, "index-levers-footer.hbs");

	private readonly HttpApiIntermediary _httpApiIntermediary;

	private readonly HttpApiUrlGenerator _httpApiUrlGenerator;

	private HandlebarsTemplate<object, object> _bodyTemplate;

	private string _footer;

	public int Order => 100;

	internal HttpLeversPageSection(HttpApiIntermediary httpApiIntermediary, HttpApiUrlGenerator httpApiUrlGenerator)
	{
		_httpApiIntermediary = httpApiIntermediary;
		_httpApiUrlGenerator = httpApiUrlGenerator;
	}

	public void Load()
	{
		_bodyTemplate = Handlebars.Compile(File.ReadAllText(BodyTemplatePath));
		_footer = Handlebars.Compile(File.ReadAllText(FooterTemplatePath))(new object());
	}

	public string BuildBody()
	{
		return _bodyTemplate(new
		{
			leversUrl = "/api/levers",
			levers = from lever in _httpApiIntermediary.GetLevers()
				orderby lever.Name
				select new
				{
					name = lever.Name,
					state = lever.State,
					isSpringReturn = lever.IsSpringReturn,
					url = "/api/levers/" + Uri.EscapeDataString(lever.Name),
					switchOnUrl = _httpApiUrlGenerator.SwitchOnLeverUrlPath(lever.Name),
					switchOffUrl = _httpApiUrlGenerator.SwitchOffLeverUrlPath(lever.Name),
					redUrl = _httpApiUrlGenerator.ColorLeverUrlPath(lever.Name, Color.red),
					greenUrl = _httpApiUrlGenerator.ColorLeverUrlPath(lever.Name, Color.green)
				}
		});
	}

	public string BuildFooter()
	{
		return _footer;
	}
}
