using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.MechanicalConnectorSystem;

internal class MechanicalConnectors : BaseComponent, IAwakableComponent
{
	private readonly Dictionary<Transput, GameObject> _connectors = new Dictionary<Transput, GameObject>();

	private readonly MaterialColorer _materialColorer;

	private EntityMaterials _entityMaterials;

	private HighlightableObject _highlightableObject;

	private BlockObject _blockObject;

	public MechanicalConnectors(MaterialColorer materialColorer)
	{
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_entityMaterials = GetComponent<EntityMaterials>();
		_highlightableObject = GetComponent<HighlightableObject>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void Add(Transput transput, GameObject connector)
	{
		_connectors.Add(transput, connector);
		Hide(connector);
		if ((bool)_entityMaterials)
		{
			_entityMaterials.AddMaterials(connector);
		}
	}

	public void Show(Transput transput)
	{
		GameObject gameObject = _connectors[transput];
		gameObject.SetActive(value: true);
		_highlightableObject.RefreshHighlight();
		if (_blockObject.IsUnfinished)
		{
			_materialColorer.EnableGrayscale(gameObject);
		}
	}

	public void Hide(Transput transput)
	{
		if (_connectors.TryGetValue(transput, out var value))
		{
			Hide(value);
		}
	}

	private static void Hide(GameObject connector)
	{
		connector.SetActive(value: false);
	}
}
