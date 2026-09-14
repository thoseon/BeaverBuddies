using Timberborn.Debugging;

namespace Timberborn.WaterSystemRendering;

internal class WaterOpacityOverrider : IDevModule
{
	private readonly WaterOpacityService _waterOpacityService;

	public WaterOpacityOverrider(WaterOpacityService waterOpacityService)
	{
		_waterOpacityService = waterOpacityService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Force water on", ToggleOpacityOverride)).Build();
	}

	private void ToggleOpacityOverride()
	{
		_waterOpacityService.ToggleOpacityOverride();
	}
}
