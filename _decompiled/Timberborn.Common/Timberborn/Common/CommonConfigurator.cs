using Bindito.Core;
using Bindito.Unity;

namespace Timberborn.Common;

[Context("Bootstrapper")]
internal class CommonConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IRandomNumberGenerator>().To<RandomNumberGenerator>().AsSingleton().AsExported();
		Bind<IFakeRandomNumberGeneratorFactory>().To<FakeRandomNumberGeneratorFactory>().AsSingleton().AsExported();
		Bind<BoundsCalculator>().AsSingleton().AsExported();
		MultiBind<ISceneInitializer>().To<InstantiatingSceneInitializer<ApplicationFocusLogger>>().AsSingleton();
	}
}
