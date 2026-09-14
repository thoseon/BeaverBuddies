using Bindito.Core;

namespace Timberborn.Intro;

[Context("MainMenu")]
internal class IntroConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IntroBox>().AsSingleton();
	}
}
