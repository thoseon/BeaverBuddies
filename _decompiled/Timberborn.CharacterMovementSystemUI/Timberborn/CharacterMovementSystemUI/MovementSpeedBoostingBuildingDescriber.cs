using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;

namespace Timberborn.CharacterMovementSystemUI;

internal class MovementSpeedBoostingBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string BonusLocKey = "Bonus.MovementSpeed";

	private readonly ILoc _loc;

	private MovementSpeedBoostingBuildingSpec _movementSpeedBoostingBuildingSpec;

	public MovementSpeedBoostingBuildingDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_movementSpeedBoostingBuildingSpec = GetComponent<MovementSpeedBoostingBuildingSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string text = $"{_loc.T(BonusLocKey)}: +{_movementSpeedBoostingBuildingSpec.BoostPercentage}%";
		yield return EntityDescription.CreateTextSection(SpecialStrings.RowStarter + text, 20);
	}
}
