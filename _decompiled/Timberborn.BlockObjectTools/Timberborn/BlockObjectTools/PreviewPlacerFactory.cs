using System;
using Timberborn.BlockSystem;

namespace Timberborn.BlockObjectTools;

public class PreviewPlacerFactory
{
	private readonly PreviewShower _previewShower;

	private readonly PreviewFactory _previewFactory;

	private readonly BlockObjectValidationService _blockObjectValidationService;

	public PreviewPlacerFactory(PreviewShower previewShower, PreviewFactory previewFactory, BlockObjectValidationService blockObjectValidationService)
	{
		_previewShower = previewShower;
		_previewFactory = previewFactory;
		_blockObjectValidationService = blockObjectValidationService;
	}

	public PreviewPlacer Create(PlaceableBlockObjectSpec placeableBlockObjectSpec)
	{
		int previewCount = placeableBlockObjectSpec.Layout.GetPreviewCount();
		Preview firstPreview = EagerlyCreateFirstPreview(placeableBlockObjectSpec);
		Lazy<Preview[]> previews = LazyCreateRemainingPreviews(placeableBlockObjectSpec, previewCount, firstPreview);
		bool treatPreviewsAsSingle = placeableBlockObjectSpec.Layout.ShouldShowAllPreviews();
		return new PreviewPlacer(_previewShower, _blockObjectValidationService, treatPreviewsAsSingle, previews);
	}

	private Preview EagerlyCreateFirstPreview(PlaceableBlockObjectSpec spec)
	{
		return _previewFactory.Create(spec);
	}

	private Lazy<Preview[]> LazyCreateRemainingPreviews(PlaceableBlockObjectSpec spec, int amount, Preview firstPreview)
	{
		return new Lazy<Preview[]>(() => CreatePreviews(spec, amount, firstPreview));
	}

	private Preview[] CreatePreviews(PlaceableBlockObjectSpec spec, int amount, Preview firstPreview)
	{
		Preview[] array = new Preview[amount];
		array[0] = firstPreview;
		for (int i = 1; i < amount; i++)
		{
			Preview preview = _previewFactory.Create(spec);
			array[i] = preview;
		}
		return array;
	}
}
