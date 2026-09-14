using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.SoilMoistureSystem;

public class DryObject : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly ISoilMoistureService _soilMoistureService;

	private BlockObject _blockObject;

	public bool IsDry { get; private set; }

	private bool IsOnMoistSoil => _soilMoistureService.SoilIsMoist(_blockObject.CoordinatesAtBaseZ);

	public event EventHandler EnteredDryState;

	public event EventHandler ExitedDryState;

	public DryObject(ISoilMoistureService soilMoistureService)
	{
		_soilMoistureService = soilMoistureService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void InitializeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			if (IsOnMoistSoil)
			{
				InternalExitDryState();
			}
			else
			{
				InternalEnterDryState();
			}
		}
	}

	public void EnterDryState()
	{
		if (!IsDry)
		{
			InternalEnterDryState();
		}
	}

	public void ExitDryState()
	{
		if (IsDry)
		{
			InternalExitDryState();
		}
	}

	private void InternalEnterDryState()
	{
		IsDry = true;
		EnteredDryState?.Invoke(this, EventArgs.Empty);
	}

	private void InternalExitDryState()
	{
		IsDry = false;
		ExitedDryState?.Invoke(this, EventArgs.Empty);
	}
}
