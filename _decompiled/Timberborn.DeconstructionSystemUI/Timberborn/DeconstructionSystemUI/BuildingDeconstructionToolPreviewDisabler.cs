using Timberborn.Debugging;

namespace Timberborn.DeconstructionSystemUI;

internal class BuildingDeconstructionToolPreviewDisabler : IDevModule
{
	private readonly BuildingDeconstructionTool _buildingDeconstructionTool;

	public BuildingDeconstructionToolPreviewDisabler(BuildingDeconstructionTool buildingDeconstructionTool)
	{
		_buildingDeconstructionTool = buildingDeconstructionTool;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle deconstruction tool preview", ToggleDeconstructionToolPreview)).Build();
	}

	private void ToggleDeconstructionToolPreview()
	{
		_buildingDeconstructionTool.TogglePreview();
	}
}
