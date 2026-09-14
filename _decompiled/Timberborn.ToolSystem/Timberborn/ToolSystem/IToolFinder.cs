using System;
using Timberborn.BaseComponentSystem;

namespace Timberborn.ToolSystem;

public interface IToolFinder
{
	bool TryFindTool(BaseComponent entity, out Action toolActivationAction);
}
