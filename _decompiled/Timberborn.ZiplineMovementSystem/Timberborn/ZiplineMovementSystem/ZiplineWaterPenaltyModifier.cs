using Timberborn.BaseComponentSystem;
using Timberborn.WalkingSystem;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineWaterPenaltyModifier : BaseComponent, IAwakableComponent, IWaterPenaltyModifier
{
	private ZiplineVisitor _ziplineVisitor;

	public float WaterPenaltyModifier
	{
		get
		{
			if (!_ziplineVisitor.IsOnZipline)
			{
				return 1f;
			}
			return 0.5f;
		}
	}

	public void Awake()
	{
		_ziplineVisitor = GetComponent<ZiplineVisitor>();
	}
}
