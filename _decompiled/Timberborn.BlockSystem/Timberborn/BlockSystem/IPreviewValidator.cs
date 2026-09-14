using Timberborn.BaseComponentSystem;
using Timberborn.Common;

namespace Timberborn.BlockSystem;

public interface IPreviewValidator
{
	bool IsValid(out string warningMessage);

	ReadOnlyHashSet<BaseComponent> InvalidatedObjects(out string warningMessage);
}
