using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.Rendering;

internal class LightingEnabler : BaseComponent, IInitializablePreview, IInitializableEntity
{
	private readonly MaterialColorer _materialColorer;

	public LightingEnabler(MaterialColorer materialColorer)
	{
		_materialColorer = materialColorer;
	}

	public void InitializePreview()
	{
		_materialColorer.EnableLighting(this);
	}

	public void InitializeEntity()
	{
		_materialColorer.EnableLighting(this);
	}
}
