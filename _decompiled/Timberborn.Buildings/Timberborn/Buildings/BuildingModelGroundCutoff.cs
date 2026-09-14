using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.Buildings;

internal class BuildingModelGroundCutoff : BaseComponent, IAwakableComponent, IInitializableEntity, IPostPlacementChangeListener
{
	private readonly MaterialHeightCutoffSetter _materialHeightCutoffSetter;

	private BuildingModelGroundCutoffSpec _buildingModelGroundCutoffSpec;

	public BuildingModelGroundCutoff(MaterialHeightCutoffSetter materialHeightCutoffSetter)
	{
		_materialHeightCutoffSetter = materialHeightCutoffSetter;
	}

	public void Awake()
	{
		_buildingModelGroundCutoffSpec = GetComponent<BuildingModelGroundCutoffSpec>();
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
		foreach (string target in _buildingModelGroundCutoffSpec.Targets)
		{
			Material[] materials = base.GameObject.FindChild(target).GetComponent<MeshRenderer>().materials;
			foreach (Material material in materials)
			{
				_materialHeightCutoffSetter.SetCutoff(material, component.WorldCenterGrounded.y + _buildingModelGroundCutoffSpec.Offset);
			}
		}
	}
}
