using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.DuplicationSystem;

internal class DuplicableInitializer : BaseComponent, IPreInitializableEntity
{
	private readonly Duplicator _duplicator;

	public DuplicableInitializer(Duplicator duplicator)
	{
		_duplicator = duplicator;
	}

	public void PreInitializeEntity()
	{
		if (TryGetComponent<DuplicationInit>(out var component))
		{
			_duplicator.Duplicate(component.DuplicationSource, this);
		}
	}
}
