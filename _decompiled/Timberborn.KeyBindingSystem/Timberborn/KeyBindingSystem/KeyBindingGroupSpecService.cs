using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.KeyBindingSystem;

public class KeyBindingGroupSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly List<KeyBindingGroupSpec> _keyBindingGroupSpecs = new List<KeyBindingGroupSpec>();

	public ReadOnlyList<KeyBindingGroupSpec> KeyBindingGroupSpecs => _keyBindingGroupSpecs.AsReadOnlyList();

	public KeyBindingGroupSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_keyBindingGroupSpecs.AddRange(GetOrderedGroups());
	}

	public bool IsHiddenGroup(string groupId)
	{
		return _keyBindingGroupSpecs.Single((KeyBindingGroupSpec group) => groupId == group.Id).IsHiddenGroup;
	}

	private IEnumerable<KeyBindingGroupSpec> GetOrderedGroups()
	{
		return from keyBindingGroupSpec in _specService.GetSpecs<KeyBindingGroupSpec>()
			orderby keyBindingGroupSpec.Order
			select keyBindingGroupSpec;
	}
}
