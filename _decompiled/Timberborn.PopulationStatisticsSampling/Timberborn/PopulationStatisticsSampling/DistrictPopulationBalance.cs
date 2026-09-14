using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.PopulationStatisticsSampling;

internal class DistrictPopulationBalance : BaseComponent, IPersistentEntity
{
	private static readonly ComponentKey DistrictPopulationBalanceKey = new ComponentKey("DistrictPopulationBalance");

	private static readonly PropertyKey<int> BirthsKey = new PropertyKey<int>("Births");

	private static readonly PropertyKey<int> DeathsKey = new PropertyKey<int>("Deaths");

	private static readonly PropertyKey<int> BotCreationsKey = new PropertyKey<int>("BotCreations");

	private static readonly PropertyKey<int> BotDestructionsKey = new PropertyKey<int>("BotDestructions");

	public int Births { get; set; }

	public int Deaths { get; set; }

	public int BotCreations { get; set; }

	public int BotDestructions { get; set; }

	public void Clear()
	{
		Births = 0;
		Deaths = 0;
		BotCreations = 0;
		BotDestructions = 0;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (Births + Deaths + BotCreations + BotDestructions > 0)
		{
			IObjectSaver component = entitySaver.GetComponent(DistrictPopulationBalanceKey);
			component.Set(BirthsKey, Births);
			component.Set(DeathsKey, Deaths);
			component.Set(BotCreationsKey, BotCreations);
			component.Set(BotDestructionsKey, BotDestructions);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DistrictPopulationBalanceKey, out var objectLoader))
		{
			Births = objectLoader.Get(BirthsKey);
			Deaths = objectLoader.Get(DeathsKey);
			BotCreations = objectLoader.Get(BotCreationsKey);
			BotDestructions = objectLoader.Get(BotDestructionsKey);
		}
	}
}
