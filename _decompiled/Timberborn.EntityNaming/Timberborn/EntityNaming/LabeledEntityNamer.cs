using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.EntityNaming;

internal class LabeledEntityNamer : BaseComponent, IAwakableComponent, IEntityNamer
{
	private LabeledEntity _labeledEntity;

	public int EntityNamerPriority => 0;

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
	}

	public string GenerateEntityName()
	{
		return _labeledEntity.DisplayName;
	}
}
