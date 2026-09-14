using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.DwellingSystem;

internal class DwellerCounter : BaseComponent, IAwakableComponent
{
	private BlockableObject _blockableObject;

	private Dwelling _dwelling;

	private DwellingStatistics? _currentDwellingStatistics;

	public event EventHandler<DwellingChangedEventArgs> DwellerCountChanged;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_dwelling = GetComponent<Dwelling>();
		_blockableObject.ObjectBlocked += OnDwellingChanged;
		_blockableObject.ObjectUnblocked += OnDwellingChanged;
		_dwelling.NumberOfDwellersChanged += OnDwellingChanged;
	}

	public DwellingStatistics GetCurrentDwellingStatistics()
	{
		DwellingStatistics valueOrDefault = _currentDwellingStatistics.GetValueOrDefault();
		if (!_currentDwellingStatistics.HasValue)
		{
			valueOrDefault = GetDwellingStatistics();
			_currentDwellingStatistics = valueOrDefault;
		}
		return _currentDwellingStatistics.Value;
	}

	private void OnDwellingChanged(object sender, EventArgs e)
	{
		DwellingStatistics dwellingStatistics = GetDwellingStatistics();
		DwellerCountChanged?.Invoke(this, new DwellingChangedEventArgs(GetCurrentDwellingStatistics(), dwellingStatistics));
		_currentDwellingStatistics = dwellingStatistics;
	}

	private DwellingStatistics GetDwellingStatistics()
	{
		if (_blockableObject.IsUnblocked)
		{
			int numberOfDwellers = _dwelling.NumberOfDwellers;
			return new DwellingStatistics(numberOfDwellers, _dwelling.MaxBeavers - numberOfDwellers);
		}
		return new DwellingStatistics(0, 0);
	}
}
