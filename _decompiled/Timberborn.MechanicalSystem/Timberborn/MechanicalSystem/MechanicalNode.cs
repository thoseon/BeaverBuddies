using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

public class MechanicalNode : BaseComponent, IAwakableComponent, IFinishedStateListener, IUnfinishedStateListener
{
	private readonly MechanicalGraphManager _mechanicalGraphManager;

	private readonly TransputMap _transputMap;

	private BlockableObject _blockableObject;

	private MechanicalNodeSpec _mechanicalNodeSpec;

	private int _nominalPowerInput;

	private int _nominalPowerOutput;

	private float _inputMultiplier = 1f;

	private bool _detached;

	private bool _addedToGraph;

	public MechanicalNodeActuals Actuals { get; private set; }

	public IBattery Battery { get; private set; }

	public MechanicalGraph Graph { get; internal set; }

	public ImmutableArray<Transput> Transputs { get; private set; }

	public bool IsShaft { get; private set; }

	public bool IsGenerator { get; private set; }

	public bool IsConsumer { get; private set; }

	public bool IsIntermediary { get; private set; }

	public bool IgnoreRotation { get; private set; }

	public bool IsDetached { get; private set; }

	public int NominalBatteryCharge { get; private set; }

	public int NominalBatteryCapacity { get; private set; }

	public float OutputMultiplier { get; private set; } = 1f;

	public bool IsBattery => Battery != null;

	public float PowerEfficiency => Graph?.PowerEfficiency ?? 0f;

	public bool Active => _blockableObject.IsUnblocked;

	public bool Powered => Graph?.Powered ?? false;

	public bool ActiveAndPowered
	{
		get
		{
			if (Active)
			{
				return Powered;
			}
			return false;
		}
	}

	public bool IsConsuming => Actuals.PowerInput > 0;

	public float NominalBatteryChargeLevel
	{
		get
		{
			if (NominalBatteryCapacity <= 0)
			{
				return 0f;
			}
			return (float)NominalBatteryCharge / (float)NominalBatteryCapacity;
		}
	}

	public event EventHandler AddedToGraph;

	public event EventHandler TransputsInitialized;

	internal MechanicalNode(MechanicalGraphManager mechanicalGraphManager, TransputMap transputMap)
	{
		_mechanicalGraphManager = mechanicalGraphManager;
		_transputMap = transputMap;
	}

	public void Awake()
	{
		_mechanicalNodeSpec = GetComponent<MechanicalNodeSpec>();
		Actuals = GetComponent<MechanicalNodeActuals>();
		Battery = GetComponent<IBattery>();
		_blockableObject = GetComponent<BlockableObject>();
		InitializeConstantParameters();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		InitializeTransputs();
		_transputMap.AddNode(this);
		AddOrRemoveFromGraph();
		InitializeActuals();
		_blockableObject.ObjectBlocked += OnBlockableObjectStateChanged;
		_blockableObject.ObjectUnblocked += OnBlockableObjectStateChanged;
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectBlocked -= OnBlockableObjectStateChanged;
		_blockableObject.ObjectUnblocked -= OnBlockableObjectStateChanged;
		RemoveFromGraph();
		_transputMap.RemoveNode(this);
		DisableComponent();
	}

	public void OnEnterUnfinishedState()
	{
		InitializeTransputs();
		_transputMap.AddNode(this);
	}

	public void OnExitUnfinishedState()
	{
		_transputMap.RemoveNode(this);
	}

	public void SetDetached(bool value)
	{
		if (IsDetached != value)
		{
			IsDetached = value;
			AddOrRemoveFromGraph();
		}
	}

	public void ReverseAllTransputs()
	{
		foreach (Transput transput in Transputs)
		{
			transput.ReverseRotation();
		}
	}

	public void ResetAllTransputRotations()
	{
		foreach (Transput transput in Transputs)
		{
			transput.ResetRotation();
		}
	}

	public void SetOutputMultiplier(float value)
	{
		if (!OutputMultiplier.Equals(value))
		{
			OutputMultiplier = value;
			UpdatePowerOutput();
		}
	}

	public void SetInputMultiplier(float value)
	{
		if (!_inputMultiplier.Equals(value))
		{
			_inputMultiplier = value;
			UpdatePowerInput();
		}
	}

	public void SetNominalBatteryCharge(int value)
	{
		if (!NominalBatteryCharge.Equals(value))
		{
			NominalBatteryCharge = value;
			UpdateBatteryCharge();
		}
	}

	public void SetNominalBatteryCapacity(int value)
	{
		if (!NominalBatteryCapacity.Equals(value))
		{
			NominalBatteryCapacity = value;
			UpdateBatteryCapacity();
		}
	}

	public IEnumerable<Transput> TransputsWithConnections()
	{
		foreach (Transput transput in Transputs)
		{
			if (transput.Connected)
			{
				yield return transput;
			}
		}
	}

	public bool CanPotentiallyBePowered()
	{
		MechanicalGraph graph = Graph;
		bool num = graph != null && graph.NumberOfGenerators > 0;
		MechanicalGraph graph2 = Graph;
		bool flag = graph2 != null && graph2.BatteryCharge > 0;
		return num | flag;
	}

	private void OnBlockableObjectStateChanged(object sender, EventArgs e)
	{
		UpdateActiveState();
	}

	private void InitializeConstantParameters()
	{
		if (_mechanicalNodeSpec != null)
		{
			IsShaft = _mechanicalNodeSpec.IsShaft;
			IsGenerator = _mechanicalNodeSpec.PowerOutput > 0;
			IsConsumer = _mechanicalNodeSpec.PowerInput > 0;
			IsIntermediary = !IsShaft && !IsGenerator && !IsConsumer;
			_nominalPowerInput = _mechanicalNodeSpec.PowerInput;
			_nominalPowerOutput = _mechanicalNodeSpec.PowerOutput;
		}
	}

	private void InitializeActuals()
	{
		if (_mechanicalNodeSpec != null)
		{
			UpdatePowerOutput();
			UpdatePowerInput();
		}
	}

	private void InitializeTransputs()
	{
		if (!(Transputs == null))
		{
			return;
		}
		BlockObject component = GetComponent<BlockObject>();
		TransputProviderSpec component2 = GetComponent<TransputProviderSpec>();
		List<Transput> list = new List<Transput>();
		foreach (TransputSpec transput in component2.Transputs)
		{
			foreach (Direction3D item2 in transput.Directions.GetEnumerator())
			{
				Transput item = new Transput(this, transput, item2, component);
				list.Add(item);
			}
		}
		Transputs = list.ToImmutableArray();
		IgnoreRotation = component2.IgnoreRotation;
		TransputsInitialized?.Invoke(this, null);
	}

	private void UpdateActiveState()
	{
		UpdatePowerOutput();
		UpdatePowerInput();
		UpdateBatteryCharge();
		UpdateBatteryCapacity();
	}

	private void UpdatePowerOutput()
	{
		if (_mechanicalNodeSpec != null)
		{
			Actuals.SetPowerOutput(Active ? Mathf.CeilToInt((float)_nominalPowerOutput * OutputMultiplier) : 0);
		}
	}

	private void UpdatePowerInput()
	{
		if (_mechanicalNodeSpec != null)
		{
			Actuals.SetPowerInput(Active ? Mathf.CeilToInt((float)_nominalPowerInput * _inputMultiplier) : 0);
		}
	}

	private void UpdateBatteryCharge()
	{
		Actuals.SetBatteryCharge(Active ? NominalBatteryCharge : 0);
	}

	private void UpdateBatteryCapacity()
	{
		Actuals.SetBatteryCapacity(Active ? NominalBatteryCapacity : 0);
	}

	private void AddOrRemoveFromGraph()
	{
		if (base.Enabled)
		{
			if (IsDetached)
			{
				RemoveFromGraph();
			}
			else
			{
				AddToGraph();
			}
		}
	}

	private void AddToGraph()
	{
		if (!_addedToGraph)
		{
			_mechanicalGraphManager.AddNode(this);
			_addedToGraph = true;
			AddedToGraph?.Invoke(this, EventArgs.Empty);
		}
	}

	private void RemoveFromGraph()
	{
		if (_addedToGraph)
		{
			_mechanicalGraphManager.RemoveNode(this);
			_addedToGraph = false;
		}
	}
}
