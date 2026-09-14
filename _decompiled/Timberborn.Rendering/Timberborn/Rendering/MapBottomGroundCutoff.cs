using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Rendering;

internal class MapBottomGroundCutoff : BaseComponent, IAwakableComponent
{
	private readonly MaterialHeightCutoffSetter _materialHeightCutoffSetter;

	public MapBottomGroundCutoff(MaterialHeightCutoffSetter materialHeightCutoffSetter)
	{
		_materialHeightCutoffSetter = materialHeightCutoffSetter;
	}

	public void Awake()
	{
		foreach (string target in GetComponent<MapBottomGroundCutoffSpec>().Targets)
		{
			MeshRenderer component = base.GameObject.FindChild(target).GetComponent<MeshRenderer>();
			_materialHeightCutoffSetter.SetCutoff(component.material, -1f);
		}
	}
}
