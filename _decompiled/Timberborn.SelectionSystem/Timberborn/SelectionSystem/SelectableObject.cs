using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class SelectableObject : BaseComponent, IAwakableComponent
{
	private ICameraTarget _cameraTarget;

	private readonly List<ISelectionListener> _selectionListenersCache = new List<ISelectionListener>();

	public Vector3 CameraTargetPosition => _cameraTarget?.CameraTargetPosition ?? base.Transform.position;

	public void Awake()
	{
		_cameraTarget = GetComponent<ICameraTarget>();
	}

	public void OnSelect()
	{
		GetComponents(_selectionListenersCache);
		foreach (ISelectionListener item in _selectionListenersCache)
		{
			item.OnSelect();
		}
		_selectionListenersCache.Clear();
	}

	public void OnUnselect()
	{
		GetComponents(_selectionListenersCache);
		foreach (ISelectionListener item in _selectionListenersCache)
		{
			item.OnUnselect();
		}
		_selectionListenersCache.Clear();
	}
}
