using System.Collections.Generic;
using System.Linq;

namespace Timberborn.MapItemsUI;

public class MapItemProvider
{
	private readonly OfficialMapItemFactory _officialMapItemFactory;

	private readonly UserMapItemFactory _userMapItemFactory;

	private readonly IEnumerable<ICustomMapItemFactory> _customMapItemFactories;

	public MapItemProvider(OfficialMapItemFactory officialMapItemFactory, UserMapItemFactory userMapItemFactory, IEnumerable<ICustomMapItemFactory> customMapItemFactories)
	{
		_officialMapItemFactory = officialMapItemFactory;
		_userMapItemFactory = userMapItemFactory;
		_customMapItemFactories = customMapItemFactories;
	}

	public IEnumerable<MapItem> GetOfficialMaps()
	{
		return _officialMapItemFactory.Create();
	}

	public IEnumerable<MapItem> GetUserMaps()
	{
		return _userMapItemFactory.Create();
	}

	public IEnumerable<MapItem> GetCustomMaps()
	{
		return GetUserMaps().Concat(_customMapItemFactories.SelectMany((ICustomMapItemFactory factory) => factory.Create()));
	}
}
