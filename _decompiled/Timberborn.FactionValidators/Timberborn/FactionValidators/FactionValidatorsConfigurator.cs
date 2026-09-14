using Bindito.Core;

namespace Timberborn.FactionValidators;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class FactionValidatorsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FactionSpecValidationService>().AsSingleton();
		MultiBind<IFactionSpecValidator>().To<FactionSpecGoodsValidator>().AsSingleton();
		MultiBind<IFactionSpecValidator>().To<FactionSpecMaterialsValidator>().AsSingleton();
		MultiBind<IFactionSpecValidator>().To<FactionSpecNeedsValidator>().AsSingleton();
		MultiBind<IFactionSpecValidator>().To<FactionSpecTemplateValidator>().AsSingleton();
	}
}
