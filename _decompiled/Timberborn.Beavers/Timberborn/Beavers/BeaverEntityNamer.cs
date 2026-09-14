using Timberborn.BaseComponentSystem;
using Timberborn.EntityNaming;

namespace Timberborn.Beavers;

internal class BeaverEntityNamer : BaseComponent, IEntityNamer
{
	private readonly BeaverNameService _beaverNameService;

	public int EntityNamerPriority => 20;

	public BeaverEntityNamer(BeaverNameService beaverNameService)
	{
		_beaverNameService = beaverNameService;
	}

	public string GenerateEntityName()
	{
		return _beaverNameService.RandomName();
	}
}
