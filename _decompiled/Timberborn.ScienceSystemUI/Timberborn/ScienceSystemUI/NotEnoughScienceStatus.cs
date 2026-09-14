using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.StatusSystem;

namespace Timberborn.ScienceSystemUI;

public class NotEnoughScienceStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string NotEnoughScienceLocKey = "Status.Science.NotEnoughScience";

	private static readonly string NotEnoughScienceShortLocKey = "Status.Science.NotEnoughScience.Short";

	private readonly ILoc _loc;

	private ScienceNeedingBuilding _scienceNeedingBuilding;

	private StatusToggle _statusToggle;

	public NotEnoughScienceStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_scienceNeedingBuilding = GetComponent<ScienceNeedingBuilding>();
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("NotEnoughScience", _loc.T(NotEnoughScienceLocKey), _loc.T(NotEnoughScienceShortLocKey), 0.2f);
		_scienceNeedingBuilding.NotEnoughScienceStateChanged += OnNotEnoughScienceStateChanged;
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	private void OnNotEnoughScienceStateChanged(object sender, NotEnoughScienceStateChangedEventArgs notEnoughScienceStateChangedEventArgs)
	{
		if (notEnoughScienceStateChangedEventArgs.NewState)
		{
			_statusToggle.Activate();
		}
		else
		{
			_statusToggle.Deactivate();
		}
	}
}
