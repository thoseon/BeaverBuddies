using Bindito.Core;
using Timberborn.CameraSystem;
using Timberborn.Debugging;

namespace Timberborn.CameraWorldState;

[Context("Game")]
[Context("MapEditor")]
internal class CameraWorldPositionConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ICameraAnchorPicker>().To<CameraAnchorPicker>().AsSingleton();
		MultiBind<IDevModule>().To<CameraWorldStateResetter>().AsSingleton();
	}
}
