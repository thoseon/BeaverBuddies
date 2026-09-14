using Timberborn.BlockSystem;
using Timberborn.Localization;

namespace Timberborn.TerrainLevelValidation;

internal class ContinuousTerrainConstraintValidator : IBlockObjectValidator
{
	private static readonly string ErrorMessageLocKey = "Buildings.ContinuousTerrainConstraint";

	private readonly ILoc _loc;

	public ContinuousTerrainConstraintValidator(ILoc loc)
	{
		_loc = loc;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		ContinuousTerrainConstraint component = blockObject.GetComponent<ContinuousTerrainConstraint>();
		if (component != null && component.IsNotOnFirstColumnOfTerrain())
		{
			errorMessage = _loc.T(ErrorMessageLocKey);
			return false;
		}
		errorMessage = null;
		return true;
	}
}
