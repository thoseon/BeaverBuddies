using Timberborn.Debugging;

namespace Timberborn.SkySystem;

internal class SkySystemDevModule : IDevModule
{
	private readonly Sun _sun;

	public SkySystemDevModule(Sun sun)
	{
		_sun = sun;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Sky: Toggle fog", ToggleFog)).Build();
	}

	private void ToggleFog()
	{
		_sun.Fog = !_sun.Fog;
	}
}
