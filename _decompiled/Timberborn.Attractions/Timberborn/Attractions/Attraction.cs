using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Effects;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.Attractions;

public class Attraction : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly FactionNeedService _factionNeedService;

	private readonly List<IBuildingEfficiencyProvider> _efficiencyProviders = new List<IBuildingEfficiencyProvider>();

	private BlockableObject _blockableObject;

	public bool SatisfiesAnyNeedToMaxValue { get; private set; }

	public IReadOnlyList<ContinuousEffectSpec> Effects { get; private set; }

	public bool IsUsable
	{
		get
		{
			if (_blockableObject.IsUnblocked)
			{
				return _efficiencyProviders.All((IBuildingEfficiencyProvider e) => e.CanUse);
			}
			return false;
		}
	}

	public Attraction(FactionNeedService factionNeedService)
	{
		_factionNeedService = factionNeedService;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		GetComponents(_efficiencyProviders);
		Effects = GetComponent<AttractionSpec>().Effects.Where((ContinuousEffectSpec spec) => _factionNeedService.IsCurrentFactionNeed(spec.NeedId)).ToList();
		SatisfiesAnyNeedToMaxValue = Effects.Any((ContinuousEffectSpec spec) => spec.SatisfyToMaxValue);
		DisableComponent();
	}

	public void GetEfficiencyAdjustedEffects(List<ContinuousEffect> continuousEffects)
	{
		float efficiency = GetEfficiency();
		for (int i = 0; i < Effects.Count; i++)
		{
			ContinuousEffectSpec continuousEffectSpec = Effects[i];
			float pointsPerHour = continuousEffectSpec.PointsPerHour * efficiency;
			continuousEffects.Add(new ContinuousEffect(continuousEffectSpec.NeedId, pointsPerHour));
		}
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private float GetEfficiency()
	{
		float num = 1f;
		for (int i = 0; i < _efficiencyProviders.Count; i++)
		{
			IBuildingEfficiencyProvider buildingEfficiencyProvider = _efficiencyProviders[i];
			num *= buildingEfficiencyProvider.Efficiency;
		}
		return num;
	}
}
