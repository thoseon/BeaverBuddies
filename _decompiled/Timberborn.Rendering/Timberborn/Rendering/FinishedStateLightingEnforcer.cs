using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Rendering;

internal class FinishedStateLightingEnforcer : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly MaterialColorer _materialColorer;

	private FinishedStateLightingEnforcerSpec _finishedStateLightingEnforcerSpec;

	public FinishedStateLightingEnforcer(MaterialColorer materialColorer)
	{
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_finishedStateLightingEnforcerSpec = GetComponent<FinishedStateLightingEnforcerSpec>();
	}

	public void OnEnterFinishedState()
	{
		foreach (string childrenName in _finishedStateLightingEnforcerSpec.ChildrenNames)
		{
			GameObject root = base.GameObject.FindChild(childrenName);
			_materialColorer.EnableLightingAndDisableChanges(this, root);
		}
	}

	public void OnExitFinishedState()
	{
	}
}
