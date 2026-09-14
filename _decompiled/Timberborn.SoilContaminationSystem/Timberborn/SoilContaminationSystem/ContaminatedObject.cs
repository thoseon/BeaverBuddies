using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.SoilContaminationSystem;

public class ContaminatedObject : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly ISoilContaminationService _soilContaminationService;

	private BlockObject _blockObject;

	public bool IsContaminated { get; private set; }

	private bool IsOnContaminatedSoil => _soilContaminationService.SoilIsContaminated(_blockObject.Coordinates);

	public event EventHandler EnteredContaminatedState;

	public event EventHandler ExitedContaminatedState;

	public ContaminatedObject(ISoilContaminationService soilContaminationService)
	{
		_soilContaminationService = soilContaminationService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void InitializeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			if (IsOnContaminatedSoil)
			{
				InternalEnterContaminatedState();
			}
			else
			{
				InternalExitContaminatedState();
			}
		}
	}

	public void EnterContaminatedState()
	{
		if (!IsContaminated)
		{
			InternalEnterContaminatedState();
		}
	}

	public void ExitContaminatedState()
	{
		if (IsContaminated)
		{
			InternalExitContaminatedState();
		}
	}

	private void InternalEnterContaminatedState()
	{
		IsContaminated = true;
		EnteredContaminatedState?.Invoke(this, EventArgs.Empty);
	}

	private void InternalExitContaminatedState()
	{
		IsContaminated = false;
		ExitedContaminatedState?.Invoke(this, EventArgs.Empty);
	}
}
