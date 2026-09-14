using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.ZiplineSystem;

public class ZiplineTowerRegistry
{
	private readonly List<ZiplineTower> _ziplineTowers = new List<ZiplineTower>();

	public ReadOnlyList<ZiplineTower> ZiplineTowers => _ziplineTowers.AsReadOnlyList();

	public void Add(ZiplineTower ziplineTower)
	{
		_ziplineTowers.Add(ziplineTower);
	}

	public void Remove(ZiplineTower ziplineTower)
	{
		_ziplineTowers.Remove(ziplineTower);
	}
}
