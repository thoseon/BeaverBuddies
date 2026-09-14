using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RecoverableGoodSystem;

[Context("Game")]
[Context("MapEditor")]
internal class RecoverableGoodSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RecoverableGoodProvider>().AsTransient();
		Bind<GoodRecoveryRateService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, RecoverableGoodProvider>();
		return builder.Build();
	}
}
