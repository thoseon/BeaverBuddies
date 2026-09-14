using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;

namespace Timberborn.BlockObjectTools;

public class PreviewPlacer
{
	private readonly PreviewShower _previewShower;

	private readonly BlockObjectValidationService _blockObjectValidationService;

	private readonly bool _treatPreviewsAsSingle;

	private readonly Lazy<Preview[]> _previews;

	private readonly List<Placement> _coordinatesCache = new List<Placement>();

	private readonly List<Preview> _buildablePreviews = new List<Preview>();

	public string WarningText { get; private set; } = "";

	private Preview[] Previews => _previews.Value;

	private Preview SinglePreview => Previews[0];

	public PreviewPlacer(PreviewShower previewShower, BlockObjectValidationService blockObjectValidationService, bool treatPreviewsAsSingle, Lazy<Preview[]> previews)
	{
		_previewShower = previewShower;
		_blockObjectValidationService = blockObjectValidationService;
		_previews = previews;
		_treatPreviewsAsSingle = treatPreviewsAsSingle;
	}

	public void ShowPreviews(IEnumerable<Placement> placements)
	{
		WarningText = string.Empty;
		_previewShower.UnhighlightAllPreviews();
		_coordinatesCache.AddRange(placements);
		if (_coordinatesCache.Count > 0)
		{
			PopulateBuildablePreviewsAndAddToBlockServices(_coordinatesCache, out var blocksCount);
			if (_treatPreviewsAsSingle)
			{
				ShowAllPreviews(_buildablePreviews);
			}
			else if (blocksCount == 1)
			{
				HideRemainingPreviews(_buildablePreviews, exceptSinglePreview: true);
				ShowSinglePreview(_buildablePreviews);
			}
			else
			{
				HideRemainingPreviews(_buildablePreviews, exceptSinglePreview: false);
				ShowBuildablePreviews(_buildablePreviews);
			}
		}
		else
		{
			HideAllPreviews();
		}
		_buildablePreviews.Clear();
		_coordinatesCache.Clear();
	}

	public IEnumerable<Placement> GetBuildableCoordinates(IEnumerable<Placement> placements)
	{
		PopulateBuildablePreviewsAndAddToBlockServices(placements, out var _);
		bool num = PreviewsAreValid(_buildablePreviews);
		RemovePreviewsFromServices();
		if (num)
		{
			foreach (Preview buildablePreview in _buildablePreviews)
			{
				yield return buildablePreview.BlockObject.Placement;
			}
		}
		_buildablePreviews.Clear();
	}

	public void HideAllPreviews()
	{
		RemovePreviewsFromServices();
		_previewShower.HidePreviews(Previews);
	}

	private bool PreviewsAreValid(IReadOnlyList<Preview> buildablePreviews)
	{
		if (_blockObjectValidationService.AreValid(buildablePreviews))
		{
			if (_treatPreviewsAsSingle)
			{
				return buildablePreviews.Count == Previews.Length;
			}
			return true;
		}
		return false;
	}

	private void PopulateBuildablePreviewsAndAddToBlockServices(IEnumerable<Placement> placements, out int blocksCount)
	{
		RemovePreviewsFromServices();
		foreach (Preview positionedBuildablePreview in GetPositionedBuildablePreviews(placements, out blocksCount))
		{
			positionedBuildablePreview.AddToPreviewServices();
			_buildablePreviews.Add(positionedBuildablePreview);
		}
	}

	private IEnumerable<Preview> GetPositionedBuildablePreviews(IEnumerable<Placement> placements, out int blocksCount)
	{
		return from preview in GetPositionedPreviews(placements, out blocksCount)
			where preview.BlockObject.IsValid()
			select preview;
	}

	private IEnumerable<Preview> GetPositionedPreviews(IEnumerable<Placement> placements, out int blocksCount)
	{
		blocksCount = 0;
		foreach (Placement placement in placements)
		{
			Previews[blocksCount++].Reposition(placement);
		}
		return Previews.Take(blocksCount);
	}

	private void RemovePreviewsFromServices()
	{
		Preview[] previews = Previews;
		for (int i = 0; i < previews.Length; i++)
		{
			previews[i].RemoveFromPreviewServices();
		}
	}

	private void ShowAllPreviews(IReadOnlyCollection<Preview> buildablePreviews)
	{
		if (buildablePreviews.Count == Previews.Length)
		{
			ShowBuildablePreviews(Previews);
		}
		else if (AnyPreviewIsAlmostValid())
		{
			ShowUnbuildablePreviews(Previews);
			WarningText = GetWarningText(Previews);
			UpdateModels(Previews);
		}
		else
		{
			HideAllPreviews();
		}
	}

	private bool AnyPreviewIsAlmostValid()
	{
		Preview[] previews = Previews;
		for (int i = 0; i < previews.Length; i++)
		{
			if (PreviewAlmostValid(previews[i]))
			{
				return true;
			}
		}
		return false;
	}

	private void ShowSinglePreview(IReadOnlyCollection<Preview> buildablePreviews)
	{
		if (buildablePreviews.Count > 0)
		{
			ShowBuildablePreviews(new Preview[1] { SinglePreview });
		}
		else if (PreviewAlmostValid(SinglePreview))
		{
			WarningText = GetWarningText(new Preview[1] { SinglePreview });
			_previewShower.ShowUnbuildablePreview(SinglePreview);
			SinglePreview.GetComponent<BlockObjectModelController>().UpdateModel();
		}
		else
		{
			HideAndRemoveFromServices(SinglePreview);
		}
	}

	private static bool PreviewAlmostValid(Preview preview)
	{
		return preview.BlockObject.IsAlmostValid();
	}

	private void HideRemainingPreviews(IReadOnlyCollection<Preview> buildablePreviews, bool exceptSinglePreview)
	{
		foreach (Preview item in Previews.Except(buildablePreviews))
		{
			if (item != SinglePreview || !exceptSinglePreview)
			{
				HideAndRemoveFromServices(item);
			}
		}
	}

	private void ShowBuildablePreviews(IReadOnlyList<Preview> previews)
	{
		if (_blockObjectValidationService.AreValid(previews, out var errorMessage))
		{
			if (_treatPreviewsAsSingle)
			{
				_previewShower.ShowBuildablePreviewsAsSingle(previews, out errorMessage);
			}
			else
			{
				_previewShower.ShowBuildablePreviews(previews, out errorMessage);
			}
		}
		else
		{
			ShowUnbuildablePreviews(previews);
		}
		UpdateModels(previews);
		WarningText = errorMessage;
	}

	private string GetWarningText(IReadOnlyList<Preview> previews)
	{
		_blockObjectValidationService.AreValid(previews, out var errorMessage);
		return errorMessage;
	}

	private void ShowUnbuildablePreviews(IReadOnlyList<Preview> previews)
	{
		if (_treatPreviewsAsSingle)
		{
			_previewShower.ShowUnbuildablePreviewsAsSingle(previews);
		}
		else
		{
			_previewShower.ShowUnbuildablePreviews(previews);
		}
	}

	private static void UpdateModels(IReadOnlyList<Preview> previews)
	{
		foreach (Preview preview in previews)
		{
			preview.GetComponent<BlockObjectModelController>().UpdateModel();
		}
	}

	private static void HideAndRemoveFromServices(Preview preview)
	{
		preview.RemoveFromPreviewServices();
		preview.Hide();
	}
}
