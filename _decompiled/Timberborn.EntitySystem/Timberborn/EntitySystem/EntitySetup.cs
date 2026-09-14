using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;

namespace Timberborn.EntitySystem;

public class EntitySetup
{
	public class Builder
	{
		private Guid? _id;

		private bool _shouldInitialize = true;

		private readonly List<object> _initComponents = new List<object>();

		public Blueprint Template { get; }

		public Builder(Blueprint template)
		{
			Template = template;
		}

		public Builder SetId(Guid id)
		{
			_id = id;
			return this;
		}

		public Builder DisableInitialization()
		{
			_shouldInitialize = false;
			return this;
		}

		public Builder AddInitComponent(object initComponent)
		{
			_initComponents.Add(initComponent);
			return this;
		}

		public EntitySetup Build()
		{
			Guid valueOrDefault = _id.GetValueOrDefault();
			if (!_id.HasValue)
			{
				valueOrDefault = Guid.NewGuid();
				_id = valueOrDefault;
			}
			return new EntitySetup(Template, _id.Value, _shouldInitialize, _initComponents);
		}
	}

	private readonly List<object> _initComponents;

	public Blueprint Template { get; }

	public Guid Id { get; }

	public bool ShouldInitialize { get; }

	public ReadOnlyList<object> InitComponents => _initComponents.AsReadOnlyList();

	private EntitySetup(Blueprint template, Guid id, bool shouldInitialize, List<object> initComponents)
	{
		Template = template;
		Id = id;
		ShouldInitialize = shouldInitialize;
		_initComponents = initComponents;
	}
}
