using Timberborn.CoreSound;
using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;

namespace Timberborn.GameSound;

internal class SoundSystemDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly CameraHeightVolumeUpdater _cameraHeightVolumeUpdater;

	public SoundSystemDebuggingPanel(DebuggingPanel debuggingPanel, CameraHeightVolumeUpdater cameraHeightVolumeUpdater)
	{
		_debuggingPanel = debuggingPanel;
		_cameraHeightVolumeUpdater = cameraHeightVolumeUpdater;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Sound system");
	}

	public string GetText()
	{
		return $"Camera height: {_cameraHeightVolumeUpdater.CameraHeight}";
	}
}
