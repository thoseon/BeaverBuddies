using Timberborn.Debugging;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.NaturalResourcesReproduction;

public class PotentialSpotsToggler : IDevModule, IUpdatableSingleton
{
	private readonly NaturalResourceReproducer _naturalResourceReproducer;

	private readonly AreaHighlightingService _areaHighlightingService;

	private bool _showingSpots;

	public PotentialSpotsToggler(NaturalResourceReproducer naturalResourceReproducer, AreaHighlightingService areaHighlightingService)
	{
		_naturalResourceReproducer = naturalResourceReproducer;
		_areaHighlightingService = areaHighlightingService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Highlight resource reproduction spots", TogglePotentialSpots)).Build();
	}

	public void UpdateSingleton()
	{
		if (_showingSpots)
		{
			DrawPotentialSpots();
		}
	}

	private void TogglePotentialSpots()
	{
		_showingSpots = !_showingSpots;
	}

	private void DrawPotentialSpots()
	{
		foreach (Vector3Int potentialSpot in _naturalResourceReproducer.PotentialSpots)
		{
			_areaHighlightingService.DrawTile(potentialSpot, Color.magenta);
		}
	}
}
