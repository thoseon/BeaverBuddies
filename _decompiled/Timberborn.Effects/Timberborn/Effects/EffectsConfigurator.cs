using Bindito.Core;

namespace Timberborn.Effects;

[Context("Game")]
internal class EffectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodEffectDescriber>().AsSingleton();
		Bind<ContinuousEffectValueSerializer>().AsSingleton();
		Bind<EffectDescriber>().AsSingleton();
	}
}
