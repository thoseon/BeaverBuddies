using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class FloodgateGateCutoff : BaseComponent, IInitializableEntity, IPostPlacementChangeListener
{
	private readonly MaterialHeightCutoffSetter _materialHeightCutoffSetter;

	public FloodgateGateCutoff(MaterialHeightCutoffSetter materialHeightCutoffSetter)
	{
		_materialHeightCutoffSetter = materialHeightCutoffSetter;
	}

	public void InitializeEntity()
	{
		UpdateCutoff();
	}

	public void OnPostPlacementChanged()
	{
		UpdateCutoff();
	}

	private void UpdateCutoff()
	{
		BlockObjectCenter component = GetComponent<BlockObjectCenter>();
		MeshRenderer componentInChildren = GetComponent<FloodgateAnimationController>().Gate.GetComponentInChildren<MeshRenderer>();
		_materialHeightCutoffSetter.SetCutoff(componentInChildren.material, component.WorldCenterGrounded.y);
	}
}
