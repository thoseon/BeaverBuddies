using Bindito.Core;
using Timberborn.CharacterMovementSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterMovementSystemUI;

[Context("Game")]
internal class CharacterMovementSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MovementSpeedBoostingBuildingDescriber>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MovementSpeedBoostingBuildingSpec, MovementSpeedBoostingBuildingDescriber>();
		return builder.Build();
	}
}
