using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.Achievements;

internal class PlaceDynamiteAtBottomTracker : BaseComponent, IAwakableComponent, IPostPlacementChangeListener
{
	private readonly PlaceDynamiteAtBottomAchievement _placeDynamiteAtBottomAchievement;

	private BlockObject _blockObject;

	public PlaceDynamiteAtBottomTracker(PlaceDynamiteAtBottomAchievement placeDynamiteAtBottomAchievement)
	{
		_placeDynamiteAtBottomAchievement = placeDynamiteAtBottomAchievement;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnPostPlacementChanged()
	{
		if (_blockObject.IsPreview && _placeDynamiteAtBottomAchievement.IsEnabled && _blockObject.CoordinatesAtBaseZ.z == 0 && base.GameObject.activeInHierarchy)
		{
			_placeDynamiteAtBottomAchievement.Unlock();
		}
	}
}
