using Bindito.Core;

namespace Timberborn.SliderToggleSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SliderToggleSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SliderToggleFactory>().AsSingleton();
		Bind<SliderToggleButtonFactory>().AsSingleton();
	}
}
