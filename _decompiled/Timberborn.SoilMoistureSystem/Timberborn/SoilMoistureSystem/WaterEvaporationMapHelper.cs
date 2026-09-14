using Timberborn.SingletonSystem;
using Timberborn.WaterSystem;

namespace Timberborn.SoilMoistureSystem;

internal class WaterEvaporationMapHelper : ILoadableSingleton, IPostLoadableSingleton
{
	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly WaterEvaporationMap _waterEvaporationMap;

	public WaterEvaporationMapHelper(IThreadSafeWaterMap threadSafeWaterMap, WaterEvaporationMap waterEvaporationMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_waterEvaporationMap = waterEvaporationMap;
	}

	public void Load()
	{
		_threadSafeWaterMap.MaxWaterColumnCountChanged += OnMaxWaterColumnCountChanged;
	}

	public void PostLoad()
	{
		_waterEvaporationMap.LoadData(_threadSafeWaterMap.ColumnCounts);
	}

	private void OnMaxWaterColumnCountChanged(object sender, int maxColumnCount)
	{
		_waterEvaporationMap.Resize(maxColumnCount);
	}
}
