using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;

namespace Timberborn.BlockSystem;

public class BlockObjectValidationService
{
	private readonly ImmutableArray<IBlockObjectValidator> _blockObjectValidators;

	public BlockObjectValidationService(IEnumerable<IBlockObjectValidator> blockObjectValidators)
	{
		_blockObjectValidators = blockObjectValidators.ToImmutableArray();
	}

	public bool IsValid(BlockObject blockObject)
	{
		foreach (IBlockObjectValidator blockObjectValidator in _blockObjectValidators)
		{
			if (!blockObjectValidator.IsValid(blockObject, out var _))
			{
				return false;
			}
		}
		return true;
	}

	public bool AreValid(IReadOnlyList<BaseComponent> previews, out string errorMessage)
	{
		foreach (BaseComponent preview in previews)
		{
			BlockObject component = preview.GetComponent<BlockObject>();
			foreach (IBlockObjectValidator blockObjectValidator in _blockObjectValidators)
			{
				if (!blockObjectValidator.IsValid(component, out errorMessage))
				{
					return false;
				}
			}
		}
		errorMessage = string.Empty;
		return true;
	}

	public bool AreValid(IReadOnlyList<BaseComponent> previews)
	{
		string errorMessage;
		return AreValid(previews, out errorMessage);
	}
}
