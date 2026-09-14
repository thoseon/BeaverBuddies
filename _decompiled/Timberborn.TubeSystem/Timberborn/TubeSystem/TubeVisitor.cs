using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeVisitor : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly TubeMap _tubeMap;

	private CharacterModel _characterModel;

	private Enterer _enterer;

	private Vector3Int _lastGridPosition;

	private Tube _currentTube;

	public TubeVisitor(TubeMap tubeMap)
	{
		_tubeMap = tubeMap;
	}

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		_enterer = GetComponent<Enterer>();
	}

	public void InitializeEntity()
	{
		UpdateVisit(_lastGridPosition = GetGridPosition());
	}

	public void DeleteEntity()
	{
		ExitTube(_currentTube);
	}

	public void UpdateVisit()
	{
		if (!_enterer.IsInside)
		{
			Vector3Int gridPosition = GetGridPosition();
			if (_lastGridPosition != gridPosition)
			{
				UpdateVisit(gridPosition);
			}
			_lastGridPosition = gridPosition;
		}
	}

	private void UpdateVisit(Vector3Int gridPosition)
	{
		Tube tubeAt = _tubeMap.GetTubeAt(gridPosition);
		Tube tube = (((bool)tubeAt && tubeAt.CanBeVisited) ? tubeAt : null);
		if (tube != _currentTube)
		{
			ExitTube(_currentTube, !tube);
			EnterTube(tube);
		}
	}

	private void ExitTube(Tube tube, bool showModel = true)
	{
		if ((bool)tube)
		{
			tube.RemoveVisitor(this);
			tube.TubeDeleted -= OnTubeDeleted;
			if (showModel)
			{
				_characterModel.Show();
			}
			_currentTube = null;
		}
	}

	private void EnterTube(Tube tube)
	{
		if ((bool)tube)
		{
			tube.AddVisitor(this);
			_characterModel.Hide();
			_currentTube = tube;
			_currentTube.TubeDeleted += OnTubeDeleted;
		}
	}

	private void OnTubeDeleted(object sender, EventArgs eventArgs)
	{
		ExitTube(_currentTube);
	}

	private Vector3Int GetGridPosition()
	{
		return NavigationCoordinateSystem.WorldToGridInt(_characterModel.Position);
	}
}
