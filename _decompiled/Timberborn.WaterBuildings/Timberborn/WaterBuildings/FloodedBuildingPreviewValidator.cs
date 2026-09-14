using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.WaterObjects;

namespace Timberborn.WaterBuildings;

internal class FloodedBuildingPreviewValidator : BaseComponent, IAwakableComponent, IPreviewValidator
{
	private static readonly string BuildingPreviewFloodedLocKey = "Buildings.PreviewFlooded";

	private static readonly ReadOnlyHashSet<BaseComponent> EmptyHashSet = new HashSet<BaseComponent>().AsReadOnlyHashSet();

	private readonly ILoc _loc;

	private FloodableObject _floodableObject;

	public FloodedBuildingPreviewValidator(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_floodableObject = GetComponent<FloodableObject>();
	}

	public bool IsValid(out string warningMessage)
	{
		warningMessage = _loc.T(BuildingPreviewFloodedLocKey);
		return !_floodableObject.IsPreviewFlooded();
	}

	public ReadOnlyHashSet<BaseComponent> InvalidatedObjects(out string warningMessage)
	{
		warningMessage = string.Empty;
		return EmptyHashSet;
	}
}
