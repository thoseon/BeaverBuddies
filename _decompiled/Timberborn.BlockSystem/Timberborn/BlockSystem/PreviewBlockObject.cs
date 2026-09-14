using Timberborn.BaseComponentSystem;

namespace Timberborn.BlockSystem;

internal class PreviewBlockObject : BaseComponent, IAwakableComponent, IPreviewServiceMember
{
	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	private bool _setInService;

	public PreviewBlockObject(PreviewBlockService previewBlockService)
	{
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void AddToPreviewService()
	{
		if (!_setInService)
		{
			_previewBlockService.SetPreview(_blockObject);
			_setInService = true;
		}
	}

	public void RemoveFromPreviewService()
	{
		if (_setInService)
		{
			_previewBlockService.UnsetPreview(_blockObject);
			_setInService = false;
		}
	}
}
