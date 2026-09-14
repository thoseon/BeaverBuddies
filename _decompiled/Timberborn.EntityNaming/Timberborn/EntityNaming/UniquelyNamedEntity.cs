using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.EntityNaming;

public class UniquelyNamedEntity : BaseComponent, IAwakableComponent, IPostInitializableEntity, IDeletableEntity
{
	private readonly UniquelyNamedEntityService _uniquelyNamedEntityService;

	private NamedEntity _namedEntity;

	private string _registeredName;

	public bool IsUnique { get; private set; }

	public string EntityName => _namedEntity.EntityName;

	public event EventHandler IsUniqueChanged;

	public event EventHandler EntityNameChanged;

	internal UniquelyNamedEntity(UniquelyNamedEntityService uniquelyNamedEntityService)
	{
		_uniquelyNamedEntityService = uniquelyNamedEntityService;
	}

	public void Awake()
	{
		_namedEntity = GetComponent<NamedEntity>();
		IsUnique = true;
	}

	public void PostInitializeEntity()
	{
		RegisterName();
		_namedEntity.EntityNameChanged += OnEntityNameChanged;
	}

	public void DeleteEntity()
	{
		_namedEntity.EntityNameChanged -= OnEntityNameChanged;
		UnregisterName();
	}

	internal void SetUnique()
	{
		SetUnique(value: true);
	}

	internal void SetNonUnique()
	{
		SetUnique(value: false);
	}

	private void SetUnique(bool value)
	{
		if (IsUnique != value)
		{
			IsUnique = value;
			IsUniqueChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnEntityNameChanged(object sender, EventArgs e)
	{
		UnregisterName();
		RegisterName();
		EntityNameChanged?.Invoke(this, EventArgs.Empty);
	}

	private void RegisterName()
	{
		_registeredName = _namedEntity.EntityName;
		if (_registeredName != null)
		{
			_uniquelyNamedEntityService.RegisterName(_registeredName, this);
		}
	}

	private void UnregisterName()
	{
		if (_registeredName != null)
		{
			_uniquelyNamedEntityService.UnregisterName(_registeredName, this);
		}
	}
}
