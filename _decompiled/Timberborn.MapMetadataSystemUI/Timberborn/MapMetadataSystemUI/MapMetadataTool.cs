using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;

namespace Timberborn.MapMetadataSystemUI;

public class MapMetadataTool : ITool, IToolDescriptor, IWaterIgnoringTool, ILoadableSingleton
{
	private static readonly string TitleLocKey = "MapEditor.MapMetadata.Title";

	private readonly ILoc _loc;

	private ToolDescription _toolDescription;

	public MapMetadataTool(ILoc loc)
	{
		_loc = loc;
	}

	public void Load()
	{
		_toolDescription = new ToolDescription.Builder(_loc.T(TitleLocKey)).Build();
	}

	public void Enter()
	{
	}

	public void Exit()
	{
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}
}
