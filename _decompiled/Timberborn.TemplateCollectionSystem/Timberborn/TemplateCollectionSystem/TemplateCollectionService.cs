using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.TemplateCollectionSystem;

public class TemplateCollectionService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly ImmutableArray<ITemplateCollectionIdProvider> _templateCollectionIdProviders;

	public ImmutableArray<Blueprint> AllTemplates { get; private set; }

	public TemplateCollectionService(ISpecService specService, IEnumerable<ITemplateCollectionIdProvider> templateCollectionIdProviders)
	{
		_specService = specService;
		_templateCollectionIdProviders = templateCollectionIdProviders.ToImmutableArray();
	}

	public void Load()
	{
		List<Blueprint> list = new List<Blueprint>();
		foreach (ITemplateCollectionIdProvider templateCollectionIdProvider in _templateCollectionIdProviders)
		{
			foreach (string collectionId in templateCollectionIdProvider.GetTemplateCollectionIds())
			{
				list.AddRange(from asset in (from spec in _specService.GetSpecs<TemplateCollectionSpec>()
						where spec.CollectionId == collectionId
						select spec).SelectMany((TemplateCollectionSpec spec) => spec.Blueprints)
					select _specService.GetBlueprint(asset.Path));
			}
		}
		AllTemplates = list.ToImmutableArray();
	}
}
