using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class ZiplineTower : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IFinishedStateListener, IPersistentEntity, IPostInitializableEntity
{
	private static readonly ComponentKey ZiplineTowerKey = new ComponentKey("ZiplineTower");

	private static readonly ListKey<ZiplineTower> ConnectionTargetsKey = new ListKey<ZiplineTower>("ConnectionTargets");

	private readonly ZiplineTowerRegistry _ziplineTowerRegistry;

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly ReferenceSerializer _referenceSerializer;

	private ZiplineTowerSpec _ziplineTowerSpec;

	private BlockObject _blockObject;

	private List<ZiplineTower> _connectionTargets;

	private List<ZiplineTower> _loadedConnectionTargets;

	public bool HasFreeSlots => _connectionTargets.Count < _ziplineTowerSpec.MaxConnections;

	public ReadOnlyList<ZiplineTower> ConnectionTargets => _connectionTargets.AsReadOnlyList();

	public bool IsActive => _blockObject.IsFinished;

	public int MaxConnections => _ziplineTowerSpec.MaxConnections;

	public int MaxDistance => _ziplineTowerSpec.MaxDistance;

	public ImmutableArray<Vector3Int> UnobstructedCoordinates => _ziplineTowerSpec.UnobstructedCoordinates;

	public Vector3 CableAnchorPoint => _blockObject.TransformCoordinates(CoordinateSystem.WorldToGrid(_ziplineTowerSpec.CableAnchorPoint));

	public Vector3Int CableAnchorPointInt => _blockObject.TransformCoordinates(CoordinateSystem.WorldToGridInt(_ziplineTowerSpec.CableAnchorPoint));

	public event EventHandler ConnectionTargetsChanged;

	public ZiplineTower(ZiplineTowerRegistry ziplineTowerRegistry, ZiplineConnectionService ziplineConnectionService, ReferenceSerializer referenceSerializer)
	{
		_ziplineTowerRegistry = ziplineTowerRegistry;
		_ziplineConnectionService = ziplineConnectionService;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_ziplineTowerSpec = GetComponent<ZiplineTowerSpec>();
		_blockObject = GetComponent<BlockObject>();
		_connectionTargets = new List<ZiplineTower>(_ziplineTowerSpec.MaxConnections);
	}

	public void InitializeEntity()
	{
		_ziplineTowerRegistry.Add(this);
	}

	public void PostInitializeEntity()
	{
		if (_loadedConnectionTargets == null)
		{
			return;
		}
		foreach (ZiplineTower loadedConnectionTarget in _loadedConnectionTargets)
		{
			if ((bool)loadedConnectionTarget && !loadedConnectionTarget.GetComponent<EntityComponent>().Deleted && !_connectionTargets.Contains(loadedConnectionTarget))
			{
				if (_ziplineConnectionService.CanBeConnected(this, loadedConnectionTarget))
				{
					_ziplineConnectionService.Connect(this, loadedConnectionTarget);
					continue;
				}
				Vector3Int coordinates = loadedConnectionTarget.GetComponent<BlockObject>().Coordinates;
				Debug.LogWarning("Unable to recreate Loaded zipline connection between" + $" {base.Name} ({_blockObject.Coordinates}) " + $"and {loadedConnectionTarget.Name} ({coordinates})");
			}
		}
	}

	public void DeleteEntity()
	{
		_ziplineTowerRegistry.Remove(this);
		while (_connectionTargets.Count > 0)
		{
			ZiplineTower otherZiplineTower = _connectionTargets[0];
			_ziplineConnectionService.Disconnect(this, otherZiplineTower);
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ZiplineTowerKey).Set(ConnectionTargetsKey, _connectionTargets, _referenceSerializer.Of<ZiplineTower>());
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ZiplineTowerKey);
		_loadedConnectionTargets = component.Get(ConnectionTargetsKey, _referenceSerializer.Of<ZiplineTower>());
	}

	public void OnEnterFinishedState()
	{
		foreach (ZiplineTower connectionTarget in _connectionTargets)
		{
			if (connectionTarget.IsActive)
			{
				_ziplineConnectionService.ActivateConnection(this, connectionTarget);
			}
		}
	}

	public void OnExitFinishedState()
	{
	}

	public bool IsConnectedTo(ZiplineTower ziplineTower)
	{
		return _connectionTargets.Contains(ziplineTower);
	}

	public void AddConnection(ZiplineTower connection)
	{
		_connectionTargets.Add(connection);
		InvokeConnectionTargetsChanged();
	}

	public void RemoveConnection(ZiplineTower connection)
	{
		_connectionTargets.Remove(connection);
		InvokeConnectionTargetsChanged();
	}

	private void InvokeConnectionTargetsChanged()
	{
		ConnectionTargetsChanged?.Invoke(this, EventArgs.Empty);
	}
}
