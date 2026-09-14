using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Timberborn.UnderstructureSystem;

namespace Timberborn.BuildingAvailability;

public class BuildingAvailabilityValidator
{
	private readonly EntityRegistry _entityRegistry;

	private readonly TemplateNameMapper _templateNameMapper;

	public BuildingAvailabilityValidator(EntityRegistry entityRegistry, TemplateNameMapper templateNameMapper)
	{
		_entityRegistry = entityRegistry;
		_templateNameMapper = templateNameMapper;
	}

	public bool IsAvailableForPlacement(ComponentSpec spec)
	{
		UnderstructureConstraintSpec spec2 = spec.GetSpec<UnderstructureConstraintSpec>();
		if ((object)spec2 != null && !AnyUnderstructureWasInstantiated(spec2))
		{
			return AnyUnderstrucuteIsBuildableByPlayer(spec2);
		}
		return true;
	}

	private bool AnyUnderstructureWasInstantiated(UnderstructureConstraintSpec understructureConstraintSpec)
	{
		return understructureConstraintSpec.UnderstructureTemplateNames.FastAny((string templateName) => _entityRegistry.WasTemplateInstantiated(templateName));
	}

	private bool AnyUnderstrucuteIsBuildableByPlayer(UnderstructureConstraintSpec understructureConstraintSpec)
	{
		return understructureConstraintSpec.UnderstructureTemplateNames.FastAny(delegate(string templateName)
		{
			if (_templateNameMapper.TryGetTemplate(templateName, out var templateSpec))
			{
				PlaceableBlockObjectSpec spec = templateSpec.GetSpec<PlaceableBlockObjectSpec>();
				if ((object)spec == null)
				{
					return false;
				}
				return !spec.DevModeTool;
			}
			return false;
		});
	}
}
