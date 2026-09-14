using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.Buildings;
using Timberborn.CharactersGame;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Workshops;

namespace Timberborn.BotUpkeep;

internal class BotManufactory : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly BotFactory _botFactory;

	private Manufactory _manufactory;

	private BuildingAccessible _buildingAccessible;

	private Enterable _enterable;

	public BotManufactory(BotFactory botFactory)
	{
		_botFactory = botFactory;
	}

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
		_manufactory = GetComponent<Manufactory>();
		_enterable = GetComponent<Enterable>();
	}

	public void InitializeEntity()
	{
		_manufactory.ProductionFinished += OnProductionFinished;
	}

	private void OnProductionFinished(object sender, EventArgs e)
	{
		CharacterBirthInit initComponent = new CharacterBirthInit(GetComponent<DistrictBuilding>().District, null, new BotManufacturedEvent());
		_botFactory.Create(_buildingAccessible.CalculateAccessFromLocalAccess(), _enterable.ExitWorldSpaceRotation, initComponent);
	}
}
