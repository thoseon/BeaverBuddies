using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ZiplineSystem;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplineTowerPreview : BaseComponent, IAwakableComponent, IPostPlacementChangeListener, IPreviewSelectionListener
{
	private readonly ConnectionCandidates _connectionCandidates;

	private ZiplineTower _ziplineTower;

	private bool _isSelected;

	public ZiplineTowerPreview(ConnectionCandidates connectionCandidates)
	{
		_connectionCandidates = connectionCandidates;
	}

	public void Awake()
	{
		_ziplineTower = GetComponent<ZiplineTower>();
	}

	public void OnPostPlacementChanged()
	{
		if (_isSelected)
		{
			_connectionCandidates.UpdateCandidates();
		}
	}

	public void OnPreviewSelect()
	{
		if (!_isSelected)
		{
			_connectionCandidates.EnableAndDrawMarkers(_ziplineTower);
			_isSelected = true;
		}
	}

	public void OnPreviewUnselect()
	{
		_connectionCandidates.Disable();
		_isSelected = false;
	}
}
