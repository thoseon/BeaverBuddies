using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ConstructionSites;

namespace Timberborn.UnderstructureSystem;

internal class UnderstructureConstructionSiteValidator : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IConstructionSiteValidator
{
	private UnderstructureConstraint _understructureConstraint;

	public bool IsValid { get; private set; }

	public bool IsModelValid => true;

	public event EventHandler ValidationStateChanged;

	public void Awake()
	{
		_understructureConstraint = GetComponent<UnderstructureConstraint>();
	}

	public void OnEnterUnfinishedState()
	{
		Validate();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void Validate()
	{
		bool isValid = IsValid;
		IsValid = _understructureConstraint.UnderstructureEntity?.GetComponent<BlockObject>().IsFinished ?? false;
		if (IsValid != isValid)
		{
			ValidationStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
