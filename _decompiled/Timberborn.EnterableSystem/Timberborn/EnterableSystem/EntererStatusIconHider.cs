using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.StatusSystem;

namespace Timberborn.EnterableSystem;

internal class EntererStatusIconHider : BaseComponent, IInitializableEntity
{
	private StatusVisibilityToggle _statusVisibilityToggle;

	public void InitializeEntity()
	{
		StatusIconCycler componentInChildren = GetComponentInChildren<StatusIconCycler>(includeInactive: true);
		_statusVisibilityToggle = componentInChildren.GetStatusVisibilityToggle();
		Enterer component = GetComponent<Enterer>();
		if (component.IsInside)
		{
			HideStatusIcons(component.CurrentBuilding);
		}
		component.EnteredEnterable += OnEnteredEnterable;
		component.ExitedEnterable += OnExitedEnterable;
	}

	private void OnEnteredEnterable(object sender, EnteredEnterableEventArgs e)
	{
		HideStatusIcons(e.Enterable);
	}

	private void OnExitedEnterable(object sender, EventArgs e)
	{
		ShowStatusIcons();
	}

	private void HideStatusIcons(Enterable enterable)
	{
		if (enterable.GetComponent<IStatusHider>() != null)
		{
			_statusVisibilityToggle.Hide();
		}
	}

	private void ShowStatusIcons()
	{
		_statusVisibilityToggle.Show();
	}
}
