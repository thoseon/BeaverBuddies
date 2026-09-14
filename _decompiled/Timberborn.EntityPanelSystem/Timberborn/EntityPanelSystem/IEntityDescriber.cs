using System.Collections.Generic;

namespace Timberborn.EntityPanelSystem;

public interface IEntityDescriber
{
	IEnumerable<EntityDescription> DescribeEntity();
}
