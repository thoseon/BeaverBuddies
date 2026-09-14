using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

public class NavMeshGroupService
{
	private static readonly int DefaultGroupId;

	private readonly Dictionary<string, int> _groupNameToId = new Dictionary<string, int>();

	private readonly List<int> _groupIds = new List<int> { DefaultGroupId };

	private int _maxId;

	public int GetOrAddGroupId(string groupName)
	{
		if (string.IsNullOrWhiteSpace(groupName))
		{
			return DefaultGroupId;
		}
		if (!_groupNameToId.TryGetValue(groupName, out var value))
		{
			value = ++_maxId;
			_groupNameToId.Add(groupName, value);
			_groupIds.Add(value);
		}
		return value;
	}

	public int GetDefaultGroupId()
	{
		return DefaultGroupId;
	}

	public ReadOnlyList<int> GetAllGroupIds()
	{
		return _groupIds.AsReadOnlyList();
	}
}
