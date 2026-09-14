using Timberborn.Debugging;
using Timberborn.ScienceSystem;

namespace Timberborn.ScienceSystemUI;

internal class ScienceAdder : IDevModule
{
	private readonly ScienceService _scienceService;

	public ScienceAdder(ScienceService scienceService)
	{
		_scienceService = scienceService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Add 1000 Science", AddScience)).Build();
	}

	private void AddScience()
	{
		_scienceService.AddPoints(1000);
	}
}
