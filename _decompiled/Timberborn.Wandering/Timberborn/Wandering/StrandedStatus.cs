using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.Wandering;

public class StrandedStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string StrandedLocKey = "Status.Homelessness.Stranded";

	private static readonly string StrandedShortLocKey = "Status.Homelessness.Stranded.Short";

	private readonly ILoc _loc;

	private Citizen _citizen;

	private Character _character;

	private StatusToggle _statusToggle;

	private bool _isDisabled;

	public StrandedStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_citizen = GetComponent<Citizen>();
		_citizen.ChangedAssignedDistrict += delegate
		{
			UpdateStatus();
		};
		_character = GetComponent<Character>();
		_character.Died += delegate
		{
			UpdateStatus();
		};
		InitializeStatus();
	}

	public void InitializeEntity()
	{
		UpdateStatus();
	}

	public void Disable()
	{
		_isDisabled = true;
		UpdateStatus();
	}

	private void InitializeStatus()
	{
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("Stranded", _loc.T(StrandedLocKey), _loc.T(StrandedShortLocKey), 0.1f);
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	private void UpdateStatus()
	{
		if (!_character.Alive || _citizen.HasAssignedDistrict || _isDisabled)
		{
			_statusToggle.Deactivate();
		}
		else
		{
			_statusToggle.Activate();
		}
	}
}
