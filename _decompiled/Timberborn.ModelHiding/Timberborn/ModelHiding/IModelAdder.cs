using Timberborn.BlockObjectModelSystem;

namespace Timberborn.ModelHiding;

public interface IModelAdder
{
	void AddModel(BlockObjectModelController model);

	void RemoveModel(BlockObjectModelController model);
}
