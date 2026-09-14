using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Explosions;
using Timberborn.Illumination;
using Timberborn.Persistence;
using Timberborn.RecoverableGoodSystem;
using Timberborn.UnderstructureSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class Detonator : BaseComponent, IAwakableComponent, IPersistentEntity, IAutomatableNeeder, ITerminal
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("Detonator");

	private static readonly PropertyKey<bool> IsArmedKey = new PropertyKey<bool>("IsArmed");

	private Automatable _automatable;

	private UnderstructureConstraint _understructureConstraint;

	private RecoverableGoodProvider _recoverableGoodProvider;

	private IlluminatorToggle _illuminatorToggle;

	private bool _isArmed;

	private float _timeWhenArmed;

	public bool NeedsAutomatable => true;

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_understructureConstraint = GetComponent<UnderstructureConstraint>();
		_recoverableGoodProvider = GetComponent<RecoverableGoodProvider>();
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_isArmed)
		{
			entitySaver.GetComponent(ComponentKey).Set(IsArmedKey, _isArmed);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader))
		{
			_isArmed = objectLoader.Get(IsArmedKey);
		}
	}

	public void Evaluate()
	{
		if (!_isArmed && _automatable.State == ConnectionState.On)
		{
			Arm();
		}
		else if (_isArmed && _automatable.State != ConnectionState.On && _timeWhenArmed.Equals(Time.time))
		{
			Disarm();
		}
	}

	private void Arm()
	{
		_isArmed = true;
		_timeWhenArmed = Time.time;
		_recoverableGoodProvider.DisableGoodRecovery();
		_illuminatorToggle.TurnOn();
		_understructureConstraint.UnderstructureEntity?.GetComponent<Dynamite>()?.Trigger();
	}

	private void Disarm()
	{
		_isArmed = false;
		_recoverableGoodProvider.EnableGoodRecovery();
		_illuminatorToggle.TurnOff();
		_understructureConstraint.UnderstructureEntity?.GetComponent<Dynamite>()?.Disarm();
	}
}
