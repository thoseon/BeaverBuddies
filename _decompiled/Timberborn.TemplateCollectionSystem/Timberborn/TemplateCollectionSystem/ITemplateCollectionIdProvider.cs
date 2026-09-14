using System.Collections.Generic;

namespace Timberborn.TemplateCollectionSystem;

public interface ITemplateCollectionIdProvider
{
	IEnumerable<string> GetTemplateCollectionIds();
}
