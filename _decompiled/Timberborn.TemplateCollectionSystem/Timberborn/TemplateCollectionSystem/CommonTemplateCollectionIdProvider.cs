using System.Collections.Generic;

namespace Timberborn.TemplateCollectionSystem;

internal class CommonTemplateCollectionIdProvider : ITemplateCollectionIdProvider
{
	private static readonly string CommonCollectionId = "Common";

	public IEnumerable<string> GetTemplateCollectionIds()
	{
		yield return CommonCollectionId;
	}
}
