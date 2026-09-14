using Bindito.Core;
using Timberborn.Timbermesh;

namespace Timberborn.TimbermeshMaterials;

[Context("Game")]
[Context("MapEditor")]
internal class TimbermeshMaterialsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IMaterialRepository>().To<MaterialRepository>().AsSingleton();
		MultiBind<IMaterialCollectionIdsProvider>().To<CommonMaterialCollectionIdsProvider>().AsSingleton();
	}
}
