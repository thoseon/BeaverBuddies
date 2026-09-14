using Bindito.Core;

namespace Timberborn.Language;

[Context("Bootstrapper")]
internal class LanguageConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LanguageSettings>().AsSingleton().AsExported();
		Bind<LanguageLoader>().AsSingleton();
	}
}
