using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.Navigation;
using Timberborn.NeedSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SoakedEffects;

internal class SoakedEffectApplier : TickableComponent, IAwakableComponent
{
	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly SoakedEffectService _soakedEffectService;

	private NeedManager _needManager;

	private IWaterResistor _waterResistor;

	public SoakedEffectApplier(IThreadSafeWaterMap threadSafeWaterMap, SoakedEffectService soakedEffectService)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_soakedEffectService = soakedEffectService;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_waterResistor = GetComponent<IWaterResistor>();
	}

	public override void Tick()
	{
		IWaterResistor waterResistor = _waterResistor;
		if (waterResistor != null && waterResistor.IsWaterResistant)
		{
			return;
		}
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(base.Transform.position);
		if (_threadSafeWaterMap.CellIsUnderwater(coordinates))
		{
			foreach (InstantEffect effect2 in _soakedEffectService.Effects)
			{
				InstantEffect effect = effect2;
				_needManager.ApplyEffect(in effect);
			}
		}
	}
}
