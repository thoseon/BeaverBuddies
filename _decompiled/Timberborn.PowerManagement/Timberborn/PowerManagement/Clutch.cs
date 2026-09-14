using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.PowerManagement;

public class Clutch : BaseComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity, IDuplicable<Clutch>, IDuplicable, IAutomatableNeeder, ITerminal
{
	private static readonly ComponentKey ClutchKey = new ComponentKey("Clutch");

	private static readonly PropertyKey<ClutchMode> ModeKey = new PropertyKey<ClutchMode>("Mode");

	private Automatable _automatable;

	private MechanicalNode _mechanicalNode;

	private bool? _wasEngaged;

	public ClutchMode Mode { get; private set; }

	public bool NeedsAutomatable => Mode == ClutchMode.Automated;

	public bool IsEngaged => Mode switch
	{
		ClutchMode.Engaged => true, 
		ClutchMode.Disengaged => false, 
		ClutchMode.Automated => _automatable.State == ConnectionState.On, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	public event EventHandler IsEngagedChanged;

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ClutchKey).Set(ModeKey, Mode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ClutchKey);
		Mode = component.Get(ModeKey);
	}

	public void InitializeEntity()
	{
		ApplyState();
	}

	public void DuplicateFrom(Clutch source)
	{
		Mode = source.Mode;
		ApplyState();
	}

	public void SetMode(ClutchMode value)
	{
		if (Mode != value)
		{
			Mode = value;
			ApplyState();
		}
	}

	public void Evaluate()
	{
		ApplyState();
	}

	private void ApplyState()
	{
		if (_wasEngaged != IsEngaged)
		{
			_mechanicalNode.SetDetached(!IsEngaged);
			IsEngagedChanged?.Invoke(this, EventArgs.Empty);
			_wasEngaged = IsEngaged;
		}
	}
}
