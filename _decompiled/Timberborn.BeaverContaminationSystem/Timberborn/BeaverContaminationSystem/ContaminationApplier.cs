using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Navigation;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.BeaverContaminationSystem;

internal class ContaminationApplier : TickableComponent, IAwakableComponent
{
	private static readonly float MinimumWaterContamination = 0.05f;

	private static readonly float ContaminationProbability = 0.01f;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private IWaterResistor _waterResistor;

	private ContaminationIncubator _contaminationIncubator;

	private Contaminable _contaminable;

	public ContaminationApplier(IThreadSafeWaterMap threadSafeWaterMap, IRandomNumberGenerator randomNumberGenerator)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_waterResistor = GetComponent<IWaterResistor>();
		_contaminationIncubator = GetComponent<ContaminationIncubator>();
		_contaminable = GetComponent<Contaminable>();
	}

	public override void Tick()
	{
		IWaterResistor waterResistor = _waterResistor;
		if ((waterResistor == null || !waterResistor.IsWaterResistant) && !_contaminationIncubator.IsIncubating && !_contaminable.IsContaminated)
		{
			TryApplyContamination();
		}
	}

	private void TryApplyContamination()
	{
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(base.Transform.position);
		if (!_threadSafeWaterMap.CellIsUnderwater(coordinates))
		{
			return;
		}
		float num = _threadSafeWaterMap.ColumnContamination(coordinates);
		if (num >= MinimumWaterContamination)
		{
			float normalizedProbability = num * ContaminationProbability;
			if (_randomNumberGenerator.CheckProbability(normalizedProbability))
			{
				_contaminationIncubator.StartIncubation();
			}
		}
	}
}
