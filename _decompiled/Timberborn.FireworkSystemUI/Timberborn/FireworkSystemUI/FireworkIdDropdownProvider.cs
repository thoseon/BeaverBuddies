using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.FireworkSystem;
using UnityEngine;

namespace Timberborn.FireworkSystemUI;

internal class FireworkIdDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedDropdownProvider, IDropdownProvider
{
	private readonly FireworkSpecService _fireworkSpecService;

	private FireworkLauncher _fireworkLauncher;

	private ImmutableArray<string> _items;

	public IReadOnlyList<string> Items => _items;

	public FireworkIdDropdownProvider(FireworkSpecService fireworkSpecService)
	{
		_fireworkSpecService = fireworkSpecService;
	}

	public void Awake()
	{
		_fireworkLauncher = GetComponent<FireworkLauncher>();
	}

	public void InitializeEntity()
	{
		_items = _fireworkSpecService.GetFireworkIds();
	}

	public string GetValue()
	{
		return _fireworkLauncher.FireworkId;
	}

	public void SetValue(string value)
	{
		_fireworkLauncher.SetFireworkId(value);
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return _fireworkSpecService.GetFireworkSpec(value).DisplayName.Value;
	}

	public Sprite GetIcon(string value)
	{
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
