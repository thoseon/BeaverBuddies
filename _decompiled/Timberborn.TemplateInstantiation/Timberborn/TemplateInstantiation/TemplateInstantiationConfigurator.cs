using Bindito.Core;

namespace Timberborn.TemplateInstantiation;

[Context("Game")]
[Context("MapEditor")]
internal class TemplateInstantiationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TemplateInstantiator>().ToProvider<TemplateInstantiatorProvider>().AsSingleton();
	}
}
