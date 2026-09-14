using System;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Illumination;

public class CustomizableIlluminator : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<CustomizableIlluminator>, IDuplicable
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("CustomizableIlluminator");

	private static readonly PropertyKey<bool> IsCustomizedKey = new PropertyKey<bool>("IsCustomized");

	private static readonly PropertyKey<Color> CustomColorKey = new PropertyKey<Color>("CustomColor");

	private readonly IlluminationService _illuminationService;

	private DefaultIlluminatorColor _defaultIlluminatorColor;

	private IlluminatorColorizer _illuminatorColorizer;

	private Color _defaultColor;

	private Color? _customColor;

	private Color? _appliedColor;

	public bool IsCustomized { get; private set; }

	public bool IsLocked { get; private set; }

	public Color CustomColor => _customColor ?? _defaultColor;

	public Color IconColor => _illuminationService.LightingColorToIconColor(EffectiveColor);

	private Color EffectiveColor
	{
		get
		{
			if (!IsCustomized)
			{
				return _defaultColor;
			}
			return CustomColor;
		}
	}

	public event EventHandler CustomColorChanged;

	public event EventHandler AppliedColorChanged;

	public CustomizableIlluminator(IlluminationService illuminationService)
	{
		_illuminationService = illuminationService;
	}

	public void Awake()
	{
		_defaultIlluminatorColor = GetComponent<DefaultIlluminatorColor>();
		_illuminatorColorizer = GetComponent<Illuminator>().CreateColorizer(100);
	}

	public void InitializeEntity()
	{
		_defaultColor = _defaultIlluminatorColor?.Color ?? _illuminationService.DefaultColor;
		Color valueOrDefault = _customColor.GetValueOrDefault();
		if (!_customColor.HasValue)
		{
			valueOrDefault = _defaultColor;
			_customColor = valueOrDefault;
		}
		Apply();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsCustomized)
		{
			IObjectSaver component = entitySaver.GetComponent(ComponentKey);
			component.Set(IsCustomizedKey, IsCustomized);
			component.Set(CustomColorKey, CustomColor);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader))
		{
			IsCustomized = objectLoader.Get(IsCustomizedKey);
			_customColor = objectLoader.Get(CustomColorKey);
		}
	}

	public void DuplicateFrom(CustomizableIlluminator source)
	{
		if (!IsLocked)
		{
			IsCustomized = source.IsCustomized;
			_customColor = source.CustomColor;
			Apply();
		}
	}

	public void SetIsCustomized(bool value)
	{
		if (IsCustomized != value)
		{
			IsCustomized = value;
			Apply();
		}
	}

	public void SetCustomColor(Color? value)
	{
		if (_customColor != value)
		{
			_customColor = value;
			Apply();
			CustomColorChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Lock()
	{
		if (!IsLocked)
		{
			IsLocked = true;
			SetCustomColor(null);
		}
	}

	public void Unlock()
	{
		if (IsLocked)
		{
			IsLocked = false;
			SetCustomColor(null);
		}
	}

	private void Apply()
	{
		Color? color = (IsCustomized ? new Color?(CustomColor) : ((Color?)null));
		if (_appliedColor != color)
		{
			if (color.HasValue)
			{
				_illuminatorColorizer.SetColor(color.Value);
			}
			else
			{
				_illuminatorColorizer.ClearColor();
			}
			_appliedColor = color;
			AppliedColorChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
