using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.CameraSystem;

[Context("Game")]
[Context("MapEditor")]
internal class CameraSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FacingCamera>().AsTransient();
		Bind<CameraFactory>().AsSingleton();
		Bind<CameraService>().AsSingleton();
		Bind<ShadowDistanceUpdater>().AsSingleton();
		Bind<CameraActionMarker>().AsSingleton();
		Bind<CameraStateRestorer>().AsSingleton();
		Bind<CameraStateSerializer>().AsSingleton();
		Bind<CameraHorizontalShifter>().AsSingleton();
		Bind<CameraMovementInput>().AsSingleton();
		Bind<GrabbingCameraTargetPicker>().AsSingleton();
		Bind<KeyboardCameraController>().AsSingleton();
		Bind<MouseCameraController>().AsSingleton();
		Bind<EdgePanningCameraTargetPicker>().AsSingleton();
		Bind<DraggingCameraTargetPicker>().AsSingleton();
		Bind<CameraAntiAliasing>().AsSingleton();
		MultiBind<IDevModule>().To<CameraSystemDevModule>().AsSingleton();
	}
}
