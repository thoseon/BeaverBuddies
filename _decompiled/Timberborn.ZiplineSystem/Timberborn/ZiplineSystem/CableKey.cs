using System;
using Timberborn.EntitySystem;

namespace Timberborn.ZiplineSystem;

internal record CableKey
{
	public ZiplineTower ZiplineTower { get; private init; }

	public ZiplineTower OtherZiplineTower { get; private init; }

	public static CableKey Create(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		Guid entityId = ziplineTower.GetComponent<EntityComponent>().EntityId;
		Guid entityId2 = otherZiplineTower.GetComponent<EntityComponent>().EntityId;
		if (entityId.CompareTo(entityId2) <= 0)
		{
			return new CableKey
			{
				ZiplineTower = otherZiplineTower,
				OtherZiplineTower = ziplineTower
			};
		}
		return new CableKey
		{
			ZiplineTower = ziplineTower,
			OtherZiplineTower = otherZiplineTower
		};
	}
}
