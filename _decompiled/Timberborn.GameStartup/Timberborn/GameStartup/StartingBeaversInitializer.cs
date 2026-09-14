using Timberborn.Beavers;
using Timberborn.NewGameConfigurationSystem;
using UnityEngine;

namespace Timberborn.GameStartup;

internal class StartingBeaversInitializer
{
	private readonly BeaverFactory _beaverFactory;

	public StartingBeaversInitializer(BeaverFactory beaverFactory)
	{
		_beaverFactory = beaverFactory;
	}

	public void Initialize(Vector3 position, int startingAdults, MinMaxSpec<float> adultAgeProgress, int startingChildren, MinMaxSpec<float> childAgeProgress)
	{
		SpawnBeavers(position, adults: true, startingAdults, adultAgeProgress);
		SpawnBeavers(position, adults: false, startingChildren, childAgeProgress);
	}

	private void SpawnBeavers(Vector3 position, bool adults, int numberOfBeavers, MinMaxSpec<float> lifeStageProgressRange)
	{
		float num = ((numberOfBeavers > 1) ? ((lifeStageProgressRange.Max - lifeStageProgressRange.Min) / (float)(numberOfBeavers - 1)) : 0f);
		for (int i = 0; i < numberOfBeavers; i++)
		{
			float lifeStageProgress = lifeStageProgressRange.Min + num * (float)i;
			SpawnBeaver(position, adults, lifeStageProgress);
		}
	}

	private void SpawnBeaver(Vector3 position, bool adult, float lifeStageProgress)
	{
		if (adult)
		{
			_beaverFactory.CreateAdult(position, lifeStageProgress);
		}
		else
		{
			_beaverFactory.CreateChild(position, lifeStageProgress);
		}
	}
}
