using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.EntityNaming;

public class NamedEntityGameObjectSynchronizer : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private TemplateSpec _templateSpec;

	private NamedEntity _namedEntity;

	public void Awake()
	{
		_templateSpec = GetComponent<TemplateSpec>();
		_namedEntity = GetComponent<NamedEntity>();
	}

	public void PostInitializeEntity()
	{
		UpdateGameObjectName();
		_namedEntity.EntityNameChanged += OnEntityNameChanged;
	}

	private void OnEntityNameChanged(object sender, EventArgs e)
	{
		UpdateGameObjectName();
	}

	private void UpdateGameObjectName()
	{
		base.GameObject.name = _templateSpec.TemplateName + " " + _namedEntity.EntityName;
	}
}
