using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Timberborn.RelationSystem;

public interface IRelationOwner
{
	event EventHandler RelationsChanged;

	IEnumerable<BaseComponent> GetRelations();
}
