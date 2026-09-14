using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplinePreviewCableRenderer : ILoadableSingleton
{
	private readonly ZiplineCableRenderer _ziplineCableRenderer;

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly RollingHighlighter _rollingHighlighter;

	private readonly ISpecService _specService;

	private readonly ZiplinePreviewTooltip _ziplinePreviewTooltip;

	private ZiplineCableModel _cableModelPreview;

	private readonly List<BlockObject> _blockingObjects = new List<BlockObject>();

	private ZiplineSystemColorsSpec _ziplineSystemColorsSpec;

	public ZiplinePreviewCableRenderer(ZiplineCableRenderer ziplineCableRenderer, ZiplineConnectionService ziplineConnectionService, RollingHighlighter rollingHighlighter, ISpecService specService, ZiplinePreviewTooltip ziplinePreviewTooltip)
	{
		_ziplineCableRenderer = ziplineCableRenderer;
		_ziplineConnectionService = ziplineConnectionService;
		_rollingHighlighter = rollingHighlighter;
		_specService = specService;
		_ziplinePreviewTooltip = ziplinePreviewTooltip;
	}

	public void Load()
	{
		_cableModelPreview = _ziplineCableRenderer.CreateCableModel();
		_ziplineSystemColorsSpec = _specService.GetSingleSpec<ZiplineSystemColorsSpec>();
		HidePreview();
	}

	public void DrawPreview(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, bool isConnectable)
	{
		if (ShouldDraw(ziplineTower, otherZiplineTower))
		{
			DrawCable(ziplineTower, otherZiplineTower, isConnectable);
			_ziplinePreviewTooltip.ShowTooltip(ziplineTower, otherZiplineTower, isConnectable);
		}
		else
		{
			HidePreview();
		}
	}

	public void HidePreview()
	{
		_cableModelPreview.SetVisibility(isVisible: false);
		_rollingHighlighter.UnhighlightAllPrimary();
		_ziplinePreviewTooltip.HideTooltip();
	}

	private static bool ShouldDraw(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if ((bool)otherZiplineTower && ziplineTower != otherZiplineTower)
		{
			return !ziplineTower.IsConnectedTo(otherZiplineTower);
		}
		return false;
	}

	private void DrawCable(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, bool isConnectable)
	{
		if (isConnectable)
		{
			DrawCable(ziplineTower, otherZiplineTower, _ziplineSystemColorsSpec.ConnectableColor);
		}
		else
		{
			DrawUnconnectableCable(ziplineTower, otherZiplineTower, _ziplineSystemColorsSpec.NotConnectableColor);
		}
	}

	private void DrawUnconnectableCable(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, Color highlightColor)
	{
		DrawCable(ziplineTower, otherZiplineTower, highlightColor);
		_ziplineConnectionService.GetBlockingObjects(ziplineTower, otherZiplineTower, _blockingObjects);
		_rollingHighlighter.HighlightPrimary(_blockingObjects, highlightColor);
		_blockingObjects.Clear();
	}

	private void DrawCable(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, Color highlightColor)
	{
		_cableModelPreview.SetVisibility(isVisible: true);
		_cableModelPreview.Highlight(highlightColor);
		_cableModelPreview.UpdateModel(ziplineTower, otherZiplineTower);
	}
}
