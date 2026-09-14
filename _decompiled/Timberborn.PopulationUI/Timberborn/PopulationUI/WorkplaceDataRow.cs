using System;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

public class WorkplaceDataRow : IPopulationRow
{
	private readonly ILoc _loc;

	private readonly Label _occupiedWorkslotCount;

	private readonly Label _freeWorkslotCount;

	private readonly Label _unemployedCount;

	private readonly Func<WorkplaceData> _workplaceDataGetter;

	private readonly Phrase _occupiedPhrase = Phrase.New().FormatCompact();

	private readonly Phrase _freePhrase = Phrase.New().FormatCompact();

	private readonly Phrase _unemployedPhrase = Phrase.New().FormatCompact();

	public WorkplaceDataRow(ILoc loc, Label occupiedWorkslotCount, Label freeWorkslotCount, Label unemployedCount, Func<WorkplaceData> workplaceDataGetter)
	{
		_loc = loc;
		_occupiedWorkslotCount = occupiedWorkslotCount;
		_freeWorkslotCount = freeWorkslotCount;
		_unemployedCount = unemployedCount;
		_workplaceDataGetter = workplaceDataGetter;
	}

	public void UpdateData()
	{
		WorkplaceData workplaceData = _workplaceDataGetter();
		_occupiedWorkslotCount.text = _loc.T(_occupiedPhrase, workplaceData.OccupiedWorkslots);
		_freeWorkslotCount.text = _loc.T(_freePhrase, workplaceData.FreeWorkslots);
		_unemployedCount.text = _loc.T(_unemployedPhrase, workplaceData.Unemployed);
	}
}
