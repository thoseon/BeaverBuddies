using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Relay : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<Relay>, IDuplicable, ICombinationalTransmitter, ITransmitter
{
	private static readonly ComponentKey RelayKey = new ComponentKey("Relay");

	private static readonly PropertyKey<RelayMode> ModeKey = new PropertyKey<RelayMode>("Mode");

	private static readonly ListKey<Automator> InputsKey = new ListKey<Automator>("Inputs");

	private static readonly PropertyKey<int> InputsCountKey = new PropertyKey<int>("InputsCount");

	private readonly ReferenceSerializer _referenceSerializer;

	private readonly List<AutomatorConnection> _inputs = new List<AutomatorConnection>();

	private Automator _automator;

	public RelayMode Mode { get; private set; }

	public ReadOnlyList<AutomatorConnection> Inputs => _inputs.AsReadOnlyList();

	public bool SupportsMultipleInputs => Mode switch
	{
		RelayMode.Not => false, 
		RelayMode.And => true, 
		RelayMode.Or => true, 
		RelayMode.Xor => true, 
		RelayMode.Passthrough => false, 
		_ => throw new ArgumentOutOfRangeException($"Unexpected value: {Mode}"), 
	};

	public Relay(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_inputs.Add(_automator.AddInput());
		_inputs.Add(_automator.AddInput());
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(RelayKey);
		component.Set(ModeKey, Mode);
		component.Set<Automator>(values: (from input in _inputs
			select input.Transmitter into transmitter
			where transmitter
			select transmitter).ToList(), key: InputsKey, serializer: _referenceSerializer.Of<Automator>());
		component.Set(InputsCountKey, _inputs.Count);
	}

	[BackwardCompatible(2026, 4, 22, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(RelayKey);
		Mode = component.Get(ModeKey);
		if (component.Has(InputsKey))
		{
			_inputs.Clear();
			List<Automator> list = component.Get(InputsKey, _referenceSerializer.Of<Automator>());
			for (int i = 0; i < list.Count; i++)
			{
				AddAndConnect(list[i], i);
			}
			for (int j = list.Count; j < component.Get(InputsCountKey); j++)
			{
				_inputs.Add(_automator.AddInput());
			}
		}
		else
		{
			PropertyKey<Automator> key = new PropertyKey<Automator>("InputA");
			PropertyKey<Automator> key2 = new PropertyKey<Automator>("InputB");
			if (component.Has(key) && component.GetObsoletable(key, _referenceSerializer.Of<Automator>(), out var value))
			{
				_inputs[0].Connect(value);
			}
			if (SupportsMultipleInputs && component.Has(key2) && component.GetObsoletable(key2, _referenceSerializer.Of<Automator>(), out var value2))
			{
				_inputs[1].Connect(value2);
			}
		}
	}

	public void DuplicateFrom(Relay source)
	{
		SetMode(source.Mode);
		_inputs.Clear();
		foreach (AutomatorConnection input in source._inputs)
		{
			AddAndConnect(input.Transmitter);
		}
		Evaluate();
	}

	public void SetMode(RelayMode relayMode)
	{
		Mode = relayMode;
		if (!SupportsMultipleInputs)
		{
			for (int i = 1; i < _inputs.Count; i++)
			{
				_inputs[i].Disconnect();
			}
		}
		Evaluate();
	}

	public Automator GetInput(int index)
	{
		if (_inputs.Count <= index)
		{
			return null;
		}
		return _inputs[index].Transmitter;
	}

	public void IncreaseInputs()
	{
		SetInput(null, _inputs.Count);
	}

	public void SetInput(Automator automator, int index)
	{
		if (index == 0 || SupportsMultipleInputs)
		{
			if (index < _inputs.Count)
			{
				_inputs[index].Connect(automator);
			}
			else
			{
				AddAndConnect(automator, index);
			}
			Evaluate();
		}
	}

	public void RemoveInput(int index)
	{
		if (index < _inputs.Count)
		{
			_inputs[index].Disconnect();
			_inputs.RemoveAt(index);
		}
	}

	public void Evaluate()
	{
		Automator automator = _automator;
		automator.SetState(Mode switch
		{
			RelayMode.Not => _inputs.Count > 0 && !_inputs[0].BooleanState, 
			RelayMode.And => AllConnectedTrue(), 
			RelayMode.Or => AnyConnectedTrue(), 
			RelayMode.Xor => OddNumberTrue(), 
			RelayMode.Passthrough => _inputs.Count > 0 && _inputs[0].BooleanState, 
			_ => throw new ArgumentOutOfRangeException($"Unexpected value: {Mode}"), 
		});
	}

	private void AddAndConnect(Automator sourceInput, int index)
	{
		for (int i = _inputs.Count; i < index; i++)
		{
			_inputs.Add(_automator.AddInput());
		}
		AddAndConnect(sourceInput);
	}

	private void AddAndConnect(Automator sourceInput)
	{
		AutomatorConnection automatorConnection = _automator.AddInput();
		automatorConnection.Connect(sourceInput);
		_inputs.Add(automatorConnection);
	}

	private bool AllConnectedTrue()
	{
		for (int i = 0; i < _inputs.Count; i++)
		{
			if (_inputs[i].IsConnected && !_inputs[i].BooleanState)
			{
				return false;
			}
		}
		return true;
	}

	private bool AnyConnectedTrue()
	{
		for (int i = 0; i < _inputs.Count; i++)
		{
			if (_inputs[i].IsConnected && _inputs[i].BooleanState)
			{
				return true;
			}
		}
		return false;
	}

	private bool OddNumberTrue()
	{
		int num = 0;
		for (int i = 0; i < _inputs.Count; i++)
		{
			if (_inputs[i].IsConnected && _inputs[i].BooleanState)
			{
				num++;
			}
		}
		return num % 2 == 1;
	}
}
