using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeSelfMarkerDrawer : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener, IPreviewSelectionListener
{
	private static readonly Color MarkerColor = new Color(0.3f, 0.5f, 0.8f, 0.75f);

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly MarkerMatrix4x4Calculator _markerMatrix4X4Calculator;

	private MeshDrawer _meshDrawer;

	private BlockObject _blockObject;

	private MechanicalNode _mechanicalNode;

	private TransputProviderSpec _transputProviderSpec;

	private BlockObjectModelController _blockObjectModelController;

	private readonly List<Matrix4x4> _markers = new List<Matrix4x4>();

	public MechanicalNodeSelfMarkerDrawer(MarkerDrawerFactory markerDrawerFactory, MarkerMatrix4x4Calculator markerMatrix4X4Calculator)
	{
		_markerDrawerFactory = markerDrawerFactory;
		_markerMatrix4X4Calculator = markerMatrix4X4Calculator;
	}

	public void Awake()
	{
		DisableComponent();
		_blockObject = GetComponent<BlockObject>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_transputProviderSpec = GetComponent<TransputProviderSpec>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_meshDrawer = (_mechanicalNode.IsGenerator ? _markerDrawerFactory.CreateMechanicalOutputMarkerDrawer(MarkerColor) : _markerDrawerFactory.CreateMechanicalInputMarkerDrawer(MarkerColor));
	}

	public void Update()
	{
		if (_blockObjectModelController.IsAnyModelShown)
		{
			if (_mechanicalNode.Transputs == null)
			{
				GetMarkersFromSpec();
			}
			_meshDrawer.DrawMultiple(_markers);
		}
	}

	public void OnSelect()
	{
		Enable();
		if (base.Enabled && _mechanicalNode.Transputs != null)
		{
			GetMarkersFromMechanicalNode();
		}
	}

	public void OnUnselect()
	{
		DisableComponent();
		_markers.Clear();
	}

	public void OnPreviewSelect()
	{
		Enable();
	}

	public void OnPreviewUnselect()
	{
		OnUnselect();
	}

	private void Enable()
	{
		if (_transputProviderSpec != null && _mechanicalNode.IsGenerator)
		{
			EnableComponent();
		}
		else
		{
			DisableComponent();
		}
	}

	private void GetMarkersFromSpec()
	{
		_markers.Clear();
		foreach (TransputSpec transput2 in _transputProviderSpec.Transputs)
		{
			foreach (Direction3D item in transput2.Directions.GetEnumerator())
			{
				Transput transput = new Transput(null, transput2, item, _blockObject);
				_markers.Add(_markerMatrix4X4Calculator.CalculateMatrixFrom(transput));
			}
		}
	}

	private void GetMarkersFromMechanicalNode()
	{
		foreach (Transput transput in _mechanicalNode.Transputs)
		{
			if (!transput.Connected)
			{
				AddMarker(transput);
			}
		}
	}

	private void AddMarker(Transput transput)
	{
		_markers.Add(_markerMatrix4X4Calculator.CalculateMatrixFrom(transput));
	}
}
