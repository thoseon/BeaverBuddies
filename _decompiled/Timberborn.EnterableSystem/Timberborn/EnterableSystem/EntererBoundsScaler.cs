using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.EnterableSystem;

internal class EntererBoundsScaler : BaseComponent, IAwakableComponent
{
	private EntererBoundsScalerSpec _entererBoundsScalerSpec;

	public void Awake()
	{
		_entererBoundsScalerSpec = GetComponent<EntererBoundsScalerSpec>();
		Enterable component = GetComponent<Enterable>();
		component.EntererAdded += delegate(object _, EntererAddedEventArgs e)
		{
			ScaleBounds(e.Enterer);
		};
		component.EntererRemoved += delegate(object _, EntererRemovedEventArgs e)
		{
			ResetBounds(e.Enterer);
		};
	}

	private void ScaleBounds(Enterer enterer)
	{
		MeshRenderer[] componentsInChildren = enterer.GameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer obj in componentsInChildren)
		{
			Bounds localBounds = obj.localBounds;
			localBounds.size *= _entererBoundsScalerSpec.Scale;
			obj.localBounds = localBounds;
		}
	}

	private static void ResetBounds(Enterer enterer)
	{
		MeshRenderer[] componentsInChildren = enterer.GameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ResetLocalBounds();
		}
	}
}
