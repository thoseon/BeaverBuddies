using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.WaterObjects;

public class FloodableObject : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private WaterObject _waterObject;

	private FloodableObjectBlockerSpec _floodableObjectBlockerSpec;

	public bool IsFlooded { get; private set; }

	public event EventHandler Flooded;

	public event EventHandler Unflooded;

	public void Awake()
	{
		_waterObject = GetComponent<WaterObject>();
		_floodableObjectBlockerSpec = GetComponent<FloodableObjectBlockerSpec>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		if (_floodableObjectBlockerSpec == null)
		{
			EnableComponent();
			UpdateFloodedState();
			_waterObject.WaterAboveBaseChanged += OnWaterAboveBaseChanged;
		}
	}

	public void OnExitFinishedState()
	{
		if (_floodableObjectBlockerSpec == null)
		{
			DisableComponent();
			_waterObject.WaterAboveBaseChanged -= OnWaterAboveBaseChanged;
		}
	}

	public bool IsPreviewFlooded()
	{
		return _waterObject.IsPreviewUnderWater();
	}

	private void OnWaterAboveBaseChanged(object sender, EventArgs e)
	{
		UpdateFloodedState();
	}

	private void UpdateFloodedState()
	{
		if (base.Enabled)
		{
			bool flag = _waterObject.WaterAboveBase > 0;
			if (!IsFlooded & flag)
			{
				Flood();
			}
			else if (IsFlooded && !flag)
			{
				Unflood();
			}
		}
	}

	private void Flood()
	{
		IsFlooded = true;
		Flooded?.Invoke(this, EventArgs.Empty);
	}

	private void Unflood()
	{
		IsFlooded = false;
		Unflooded?.Invoke(this, EventArgs.Empty);
	}
}
