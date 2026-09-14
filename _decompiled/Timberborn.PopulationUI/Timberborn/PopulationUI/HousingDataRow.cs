using System;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

public class HousingDataRow : IPopulationRow
{
	private readonly ILoc _loc;

	private readonly Label _occupiedBedCount;

	private readonly Label _freeBedCount;

	private readonly Label _homelessCount;

	private readonly Func<PopulationData> _populationDataGetter;

	private readonly Phrase _occupiedPhrase = Phrase.New().FormatCompact();

	private readonly Phrase _freePhrase = Phrase.New().FormatCompact();

	private readonly Phrase _homelessPhrase = Phrase.New().FormatCompact();

	public HousingDataRow(ILoc loc, Label occupiedBedCount, Label freeBedCount, Label homelessCount, Func<PopulationData> populationDataGetter)
	{
		_loc = loc;
		_occupiedBedCount = occupiedBedCount;
		_freeBedCount = freeBedCount;
		_homelessCount = homelessCount;
		_populationDataGetter = populationDataGetter;
	}

	public void UpdateData()
	{
		BedData bedData = _populationDataGetter().BedData;
		_occupiedBedCount.text = _loc.T(_occupiedPhrase, bedData.OccupiedBeds);
		_freeBedCount.text = _loc.T(_freePhrase, bedData.FreeBeds);
		_homelessCount.text = _loc.T(_homelessPhrase, bedData.Homeless);
	}
}
