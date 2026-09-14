using Bindito.Core;

namespace Timberborn.LanguageUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class LanguageUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ChangeLanguageBox>().AsSingleton();
	}
}
