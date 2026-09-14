using Bindito.Core;
using Timberborn.Automation;
using Timberborn.EntityNaming;
using Timberborn.TemplateInstantiation;

namespace Timberborn.HttpApiSystem;

[Context("Game")]
internal class HttpApiSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HttpAdapter>().AsTransient();
		Bind<HttpLever>().AsTransient();
		Bind<HttpApiController>().AsTransient();
		Bind<HttpApi>().AsSingleton();
		Bind<HttpApiIntermediary>().AsSingleton();
		Bind<HttpWebhookCaller>().AsSingleton();
		Bind<HttpWebhookRegistry>().AsSingleton();
		Bind<HttpApiCacheBuster>().AsSingleton();
		Bind<HttpApiUrlGenerator>().AsTransient();
		MultiBind<IHttpApiEndpoint>().To<IndexHtmlEndpoint>().AsSingleton();
		MultiBind<IHttpApiEndpoint>().To<StaticFilesEndpoint>().AsSingleton();
		MultiBind<IHttpApiEndpoint>().To<HttpAdaptersJsonEndpoint>().AsSingleton();
		MultiBind<IHttpApiEndpoint>().To<HttpLeverJsonEndpoint>().AsSingleton();
		MultiBind<IHttpApiPageSection>().To<HttpLeversPageSection>().AsSingleton();
		MultiBind<IHttpApiPageSection>().To<HttpAdapterPageSection>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<HttpAdapterSpec, HttpAdapter>();
		builder.AddDecorator<HttpAdapter, HttpApiController>();
		builder.AddDecorator<HttpAdapter, AutomatorIlluminator>();
		builder.AddDecorator<HttpAdapter, NumberedEntityNamer>();
		builder.AddDecorator<HttpAdapter, UniquelyNamedEntity>();
		builder.AddDecorator<HttpAdapter, Automatable>();
		builder.AddDecorator<HttpLeverSpec, HttpLever>();
		builder.AddDecorator<HttpLever, HttpApiController>();
		builder.AddDecorator<HttpLever, AutomatorIlluminator>();
		builder.AddDecorator<HttpLever, UniquelyNamedEntity>();
		return builder.Build();
	}
}
