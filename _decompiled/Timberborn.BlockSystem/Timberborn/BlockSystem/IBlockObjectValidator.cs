namespace Timberborn.BlockSystem;

public interface IBlockObjectValidator
{
	bool IsValid(BlockObject blockObject, out string errorMessage);
}
