using Bindito.Core;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DecalSystem;

[Context("Game")]
internal class DecalSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DecalSupplier>().AsTransient();
		Bind<DecalSupplierBuildingIcon>().AsTransient();
		Bind<FlippableDecal>().AsTransient();
		Bind<IDecalService>().To<DecalService>().AsSingleton();
		Bind<UserDecalService>().AsSingleton();
		Bind<UserDecalTextureRepository>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DecalSupplierBuildingIconSpec, DecalSupplierBuildingIcon>();
		builder.AddDecorator<DecalSupplierBuildingIcon, EntityMaterials>();
		builder.AddDecorator<DecalSupplierSpec, DecalSupplier>();
		builder.AddDecorator<FlippableDecalSpec, FlippableDecal>();
		return builder.Build();
	}
}
