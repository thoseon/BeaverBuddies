using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockingSystem;

internal class BlockableObjectVisualizer : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private BlockableObject _blockableObject;

	private GameObject _hideableObject;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		string hideableObjectName = GetComponent<BlockableObjectVisualizerSpec>().HideableObjectName;
		_hideableObject = base.GameObject.FindChild(hideableObjectName);
		DisableComponent();
		UpdateVisualization();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		UpdateVisualization();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		UpdateVisualization();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		UpdateVisualization();
	}

	private void UpdateVisualization()
	{
		_hideableObject.SetActive(_blockableObject.IsUnblocked && base.Enabled);
	}
}
