using Timberborn.BlockSystem;

namespace Timberborn.UnderstructureSystem;

internal class UnderstructureConstraintValidator : IBlockObjectValidator
{
	private readonly UnderstructureFinder _understructureFinder;

	public UnderstructureConstraintValidator(UnderstructureFinder understructureFinder)
	{
		_understructureFinder = understructureFinder;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		UnderstructureConstraint component = blockObject.GetComponent<UnderstructureConstraint>();
		if (component != null && !_understructureFinder.FindStrict(blockObject, component))
		{
			errorMessage = component.ErrorMessage;
			return false;
		}
		errorMessage = null;
		return true;
	}
}
