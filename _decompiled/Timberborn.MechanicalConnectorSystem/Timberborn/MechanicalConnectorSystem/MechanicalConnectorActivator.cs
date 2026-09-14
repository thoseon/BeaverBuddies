using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;

namespace Timberborn.MechanicalConnectorSystem;

internal class MechanicalConnectorActivator : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IFinishedStateListener
{
	private readonly MechanicalConnectorFactory _mechanicalConnectorFactory;

	private readonly TransputMap _transputMap;

	private MechanicalNode _mechanicalNode;

	private BuildingModel _buildingModel;

	private MechanicalConnectors _mechanicalConnectors;

	private readonly List<Transput> _connectableTransputs = new List<Transput>();

	private bool _isInitialized;

	public MechanicalConnectorActivator(MechanicalConnectorFactory mechanicalConnectorFactory, TransputMap transputMap)
	{
		_mechanicalConnectorFactory = mechanicalConnectorFactory;
		_transputMap = transputMap;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_buildingModel = GetComponent<BuildingModel>();
		_mechanicalConnectors = GetComponent<MechanicalConnectors>();
	}

	public void OnEnterUnfinishedState()
	{
		InitializeTransputs();
		_transputMap.TransputAdded += OnTransputAdded;
		_transputMap.TransputRemoved += OnTransputRemoved;
	}

	public void OnExitUnfinishedState()
	{
		_transputMap.TransputAdded -= OnTransputAdded;
		_transputMap.TransputRemoved -= OnTransputRemoved;
	}

	public void OnEnterFinishedState()
	{
		InitializeTransputs();
		_transputMap.TransputAdded += OnTransputAdded;
		_transputMap.TransputRemoved += OnTransputRemoved;
	}

	public void OnExitFinishedState()
	{
		_transputMap.TransputAdded -= OnTransputAdded;
		_transputMap.TransputRemoved -= OnTransputRemoved;
	}

	private void InitializeTransputs()
	{
		if (_isInitialized)
		{
			return;
		}
		_connectableTransputs.AddRange(_mechanicalNode.Transputs.Where(IsConnectable));
		foreach (Transput connectableTransput in _connectableTransputs)
		{
			InitializeTransput(connectableTransput);
		}
		_isInitialized = true;
	}

	private void OnTransputAdded(object sender, Transput other)
	{
		foreach (Transput connectableTransput in _connectableTransputs)
		{
			if (connectableTransput.Faces(other))
			{
				ShowConnector(connectableTransput, other);
				break;
			}
		}
	}

	private void OnTransputRemoved(object sender, Transput other)
	{
		foreach (Transput connectableTransput in _connectableTransputs)
		{
			if (connectableTransput.Faces(other))
			{
				_mechanicalConnectors.Hide(connectableTransput);
				break;
			}
		}
	}

	private void InitializeTransput(Transput transput)
	{
		_mechanicalConnectorFactory.Create(transput, _buildingModel.FinishedModel.transform, _mechanicalConnectors);
		Transput facingTransput = _transputMap.GetFacingTransput(transput);
		if (facingTransput != null)
		{
			ShowConnector(transput, facingTransput);
		}
	}

	private void ShowConnector(Transput transput, Transput target)
	{
		MechanicalNode parentNode = target.ParentNode;
		if (parentNode.IsShaft || parentNode.IsGenerator || parentNode.IsIntermediary)
		{
			_mechanicalConnectors.Show(transput);
		}
	}

	private static bool IsConnectable(Transput transput)
	{
		if (!transput.Direction.IsHorizontal())
		{
			return transput.Direction == Direction3D.Top;
		}
		return true;
	}
}
