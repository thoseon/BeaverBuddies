using Bindito.Core;
using Timberborn.WellbeingUI;

namespace Timberborn.MortalSystemUI;

[Context("Game")]
internal class MortalSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<INeedEffectDescriber>().To<LethalNeedEffectDescriber>().AsSingleton();
	}
}
