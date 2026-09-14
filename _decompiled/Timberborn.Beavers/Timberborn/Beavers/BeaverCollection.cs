using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Beavers;

public class BeaverCollection
{
	private readonly List<Beaver> _beavers = new List<Beaver>();

	private readonly List<Beaver> _adults = new List<Beaver>();

	private readonly List<Beaver> _children = new List<Beaver>();

	public ReadOnlyList<Beaver> Beavers => _beavers.AsReadOnlyList();

	public ReadOnlyList<Beaver> Adults => _adults.AsReadOnlyList();

	public ReadOnlyList<Beaver> Children => _children.AsReadOnlyList();

	public int NumberOfBeavers => Beavers.Count;

	public int NumberOfAdults => Adults.Count;

	public int NumberOfChildren => Children.Count;

	public void AddBeaver(Beaver beaver)
	{
		if (beaver.HasComponent<ChildSpec>())
		{
			_children.Add(beaver);
		}
		else
		{
			_adults.Add(beaver);
		}
		_beavers.Add(beaver);
	}

	public void RemoveBeaver(Beaver beaver)
	{
		_adults.Remove(beaver);
		_children.Remove(beaver);
		_beavers.Remove(beaver);
	}
}
