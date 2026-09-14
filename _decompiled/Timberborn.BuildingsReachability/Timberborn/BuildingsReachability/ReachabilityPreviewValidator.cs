using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Localization;

namespace Timberborn.BuildingsReachability;

internal class ReachabilityPreviewValidator : BaseComponent, IAwakableComponent, IPreviewValidator
{
	private static readonly ReadOnlyHashSet<BaseComponent> EmptyObjects = new HashSet<BaseComponent>().AsReadOnlyHashSet();

	private static readonly string UnreachableObjectLocKey = "Status.Object.UnreachableObject";

	private readonly ILoc _loc;

	private ReachableConstructionSite _reachableConstructionSite;

	public ReachabilityPreviewValidator(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_reachableConstructionSite = GetComponent<ReachableConstructionSite>();
	}

	public bool IsValid(out string warningMessage)
	{
		warningMessage = _loc.T(UnreachableObjectLocKey);
		return !_reachableConstructionSite.IsUnreachable();
	}

	public ReadOnlyHashSet<BaseComponent> InvalidatedObjects(out string warningMessage)
	{
		warningMessage = null;
		return EmptyObjects;
	}
}
