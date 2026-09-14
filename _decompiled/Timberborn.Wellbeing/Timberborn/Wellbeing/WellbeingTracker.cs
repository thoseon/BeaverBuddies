using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.Wellbeing;

public class WellbeingTracker : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private NeedManager _needManager;

	public int Wellbeing { get; private set; }

	public event EventHandler<WellbeingChangedEventArgs> WellbeingChanged;

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
	}

	public void InitializeEntity()
	{
		_needManager.NeedChangedActiveState += OnNeedChangedActiveState;
		_needManager.NeedChangedIsFavorable += OnNeedChangedIsFavorable;
		UpdateWellbeing();
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		UpdateWellbeing();
	}

	private void OnNeedChangedIsFavorable(object sender, NeedChangedIsFavorableEventArgs e)
	{
		UpdateWellbeing();
	}

	private void UpdateWellbeing()
	{
		int wellbeing = Wellbeing;
		Wellbeing = 0;
		for (int i = 0; i < _needManager.NeedSpecs.Length; i++)
		{
			NeedSpec needSpec = _needManager.NeedSpecs[i];
			Wellbeing += _needManager.GetNeedWellbeing(needSpec.Id);
		}
		if (wellbeing != Wellbeing)
		{
			WellbeingChanged?.Invoke(this, new WellbeingChangedEventArgs(wellbeing, Wellbeing));
		}
	}
}
