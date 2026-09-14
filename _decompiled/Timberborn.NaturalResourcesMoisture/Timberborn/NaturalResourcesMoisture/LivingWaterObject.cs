using System;
using Timberborn.BaseComponentSystem;
using Timberborn.WaterObjects;
using UnityEngine;

namespace Timberborn.NaturalResourcesMoisture;

public class LivingWaterObject : BaseComponent, IAwakableComponent, IWaterObjectSpecification
{
	private WaterObject _waterObject;

	private FloodableNaturalResourceSpec _floodableNaturalResourceSpec;

	private bool _initialized;

	public bool WaterNeedsAreMet { get; private set; }

	public Vector3Int WaterCoordinates => Vector3Int.zero;

	private bool Dry => _waterObject.WaterAboveBase < _floodableNaturalResourceSpec.MinWaterHeight;

	private bool Flooded => _waterObject.WaterAboveBase > _floodableNaturalResourceSpec.MaxWaterHeight;

	public event EventHandler<WaterNeedsUnmetEventArgs> WaterNeedsUnmet;

	public event EventHandler WaterNeedsMet;

	public void Awake()
	{
		_waterObject = GetComponent<WaterObject>();
		_floodableNaturalResourceSpec = GetComponent<FloodableNaturalResourceSpec>();
		_waterObject.WaterAboveBaseChanged += delegate
		{
			CheckWaterNeeds();
		};
	}

	private void CheckWaterNeeds()
	{
		bool flag = !Dry && !Flooded;
		if (flag && (!WaterNeedsAreMet || !_initialized))
		{
			WaterNeedsAreMet = true;
			WaterNeedsMet?.Invoke(this, EventArgs.Empty);
		}
		else if (!flag && (WaterNeedsAreMet || !_initialized))
		{
			WaterNeedsAreMet = false;
			WaterNeedsUnmet?.Invoke(this, new WaterNeedsUnmetEventArgs(Flooded));
		}
		_initialized = true;
	}
}
