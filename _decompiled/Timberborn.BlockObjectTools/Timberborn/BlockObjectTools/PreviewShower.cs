using System.Collections.Generic;
using System.Text;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

public class PreviewShower : ILoadableSingleton
{
	private readonly Highlighter _highlighter;

	private readonly ISpecService _specService;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private readonly HashSet<BaseComponent> _invalidatedObjects = new HashSet<BaseComponent>();

	private readonly List<IPreviewValidator> _previewValidatorsCache = new List<IPreviewValidator>();

	private readonly List<IPrePreviewShownListener> _prePreviewShownListeners = new List<IPrePreviewShownListener>();

	private readonly List<Vector3Int> _areaCoordinates = new List<Vector3Int>();

	private PreviewShowerSpec _previewShowerSpec;

	public PreviewShower(Highlighter highlighter, ISpecService specService, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_highlighter = highlighter;
		_specService = specService;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public void Load()
	{
		_previewShowerSpec = _specService.GetSingleSpec<PreviewShowerSpec>();
	}

	public void ShowBuildablePreviewsAsSingle(IReadOnlyList<Preview> previews, out string warningMessage)
	{
		ShowBuildablePreviews(previews, showAsSingle: true, out warningMessage);
	}

	public void ShowBuildablePreviews(IReadOnlyList<Preview> previews, out string warningMessage)
	{
		ShowBuildablePreviews(previews, previews.Count == 1, out warningMessage);
	}

	public void ShowUnbuildablePreview(Preview preview)
	{
		ShowPreview(preview, PreviewState.UnbuildableSingle, _previewShowerSpec.UnbuildablePreview);
	}

	public void ShowUnbuildablePreviewsAsSingle(IReadOnlyList<Preview> previews)
	{
		ShowUnbuildablePreviews(previews, showAsSingle: true);
	}

	public void ShowUnbuildablePreviews(IReadOnlyList<Preview> previews)
	{
		ShowUnbuildablePreviews(previews, previews.Count == 1);
	}

	public void UnhighlightAllPreviews()
	{
		_highlighter.UnhighlightAllPrimary();
	}

	public void HidePreviews(IEnumerable<Preview> previews)
	{
		UnhighlightAllPreviews();
		foreach (Preview preview in previews)
		{
			preview.Hide();
		}
	}

	private void ShowBuildablePreviews(IReadOnlyList<Preview> previews, bool showAsSingle, out string warningMessage)
	{
		HashSet<string> warningMessages = new HashSet<string>();
		for (int i = 0; i < previews.Count; i++)
		{
			Preview preview = previews[i];
			PreviewState previewState = (showAsSingle ? PreviewState.BuildableSingle : ((i == previews.Count - 1) ? PreviewState.BuildableLast : PreviewState.BuildableNotLast));
			ShowBuildablePreview(preview, previewState, warningMessages);
			if (!showAsSingle)
			{
				_areaCoordinates.AddRange(preview.BlockObject.PositionedBlocks.GetOccupiedCoordinates());
			}
		}
		foreach (BaseComponent invalidatedObject in _invalidatedObjects)
		{
			_highlighter.HighlightPrimary(invalidatedObject, _previewShowerSpec.WarningPreview);
		}
		_measurableAreaDrawer.AddMeasurableCoordinates(_areaCoordinates);
		_invalidatedObjects.Clear();
		_areaCoordinates.Clear();
		warningMessage = BuildWarningMessage(warningMessages);
	}

	private void ShowBuildablePreview(Preview preview, PreviewState previewState, HashSet<string> warningMessages)
	{
		Color color = _previewShowerSpec.BuildablePreview;
		preview.GetComponents(_previewValidatorsCache);
		foreach (IPreviewValidator item in _previewValidatorsCache)
		{
			if (PreviewIsInvalid(item, out var warningMessage))
			{
				warningMessages.Add(warningMessage);
				color = _previewShowerSpec.WarningPreview;
			}
		}
		_previewValidatorsCache.Clear();
		ShowPreview(preview, previewState, color);
	}

	private bool PreviewIsInvalid(IPreviewValidator previewValidator, out string warningMessage)
	{
		bool flag = CheckAndAddInvalidatedObjects(previewValidator, out var warningMessage2);
		if (!previewValidator.IsValid(out var warningMessage3))
		{
			warningMessage = warningMessage3;
			return true;
		}
		if (flag)
		{
			warningMessage = warningMessage2;
			return true;
		}
		warningMessage = null;
		return false;
	}

	private bool CheckAndAddInvalidatedObjects(IPreviewValidator previewValidator, out string warningMessage)
	{
		bool result = false;
		foreach (BaseComponent item in previewValidator.InvalidatedObjects(out warningMessage))
		{
			_invalidatedObjects.Add(item);
			result = true;
		}
		return result;
	}

	private static string BuildWarningMessage(IEnumerable<string> warningMessages)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string warningMessage in warningMessages)
		{
			stringBuilder.AppendLine(warningMessage);
		}
		return stringBuilder.ToStringWithoutNewLineEnd();
	}

	private void ShowUnbuildablePreviews(IReadOnlyList<Preview> previews, bool showAsSingle)
	{
		for (int i = 0; i < previews.Count; i++)
		{
			Preview preview = previews[i];
			PreviewState previewState = (showAsSingle ? PreviewState.UnbuildableSingle : ((i == previews.Count - 1) ? PreviewState.UnbuildableLast : PreviewState.UnbuildableNotLast));
			ShowPreview(preview, previewState, _previewShowerSpec.UnbuildablePreview);
		}
	}

	private void ShowPreview(Preview preview, PreviewState previewState, Color color)
	{
		NotifyPrePreviewShownListeners(preview);
		preview.Show(previewState);
		_highlighter.HighlightPrimary(preview, color);
	}

	private void NotifyPrePreviewShownListeners(Preview preview)
	{
		preview.GetComponents(_prePreviewShownListeners);
		foreach (IPrePreviewShownListener prePreviewShownListener in _prePreviewShownListeners)
		{
			prePreviewShownListener.OnPrePreviewShown();
		}
		_prePreviewShownListeners.Clear();
	}
}
