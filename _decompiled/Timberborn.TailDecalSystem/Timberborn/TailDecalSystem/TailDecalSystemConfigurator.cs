using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TailDecalSystem;

[Context("Game")]
internal class TailDecalSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EnterableTailDecalApplier>().AsTransient();
		Bind<TailDecalApplier>().AsTransient();
		Bind<TailDecalTextureSetter>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, TailDecalApplier>();
		builder.AddDecorator<TailDecalApplier, TailDecalTextureSetter>();
		builder.AddDecorator<EnterableTailDecalApplierSpec, EnterableTailDecalApplier>();
		return builder.Build();
	}
}
