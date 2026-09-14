using System.Collections.Generic;
using JetBrains.Annotations;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.Illumination;

public class Illuminator : BaseComponent, IAwakableComponent, IPreInitializableEntity
{
	private static readonly float DefaultStrength = 1f;

	private readonly IlluminationService _illuminationService;

	private readonly MaterialColorer _materialColorer;

	private IlluminatorLightObjects _illuminatorLightObjects;

	private float _strength = DefaultStrength;

	private bool _isOn;

	private int _turnedOnToggles;

	private int _disabledToggles;

	private readonly SortedList<int, Color> _colors = new SortedList<int, Color>();

	private Color? _topColor;

	public Illuminator(IlluminationService illuminationService, MaterialColorer materialColorer)
	{
		_illuminationService = illuminationService;
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_illuminatorLightObjects = GetComponent<IlluminatorLightObjects>();
	}

	public void PreInitializeEntity()
	{
		UpdateColor();
		UpdateState();
	}

	public IlluminatorToggle CreateToggle()
	{
		return new IlluminatorToggle(this);
	}

	public IlluminatorColorizer CreateColorizer(int priority)
	{
		return new IlluminatorColorizer(this, priority);
	}

	[UsedImplicitly]
	public void SetStrength(float strength)
	{
		_strength = strength;
		UpdateStrength();
	}

	internal void IncrementTurnedOnToggles()
	{
		if (_turnedOnToggles++ == 0)
		{
			UpdateState();
		}
	}

	internal void DecrementTurnedOnToggles()
	{
		if (--_turnedOnToggles == 0)
		{
			UpdateState();
		}
	}

	internal void IncrementDisabledToggles()
	{
		if (_disabledToggles++ == 0)
		{
			UpdateState();
		}
	}

	internal void DecrementDisabledToggles()
	{
		if (--_disabledToggles == 0)
		{
			UpdateState();
		}
	}

	internal void SetColor(int priority, Color color)
	{
		_colors[priority] = color;
		UpdateColor();
	}

	internal void ClearColor(int priority)
	{
		if (_colors.Remove(priority))
		{
			UpdateColor();
		}
	}

	private void UpdateState()
	{
		_isOn = _turnedOnToggles > 0 && _disabledToggles == 0;
		UpdateStrength();
	}

	private void UpdateStrength()
	{
		if ((bool)_illuminatorLightObjects)
		{
			_illuminatorLightObjects.SetActive(_isOn);
		}
		if (_isOn)
		{
			_materialColorer.EnableLighting(this, _strength);
		}
		else
		{
			_materialColorer.DisableLighting(this);
		}
	}

	private void UpdateColor()
	{
		Color color = (_colors.IsEmpty() ? _illuminationService.DefaultColor : _colors.Values[_colors.Count - 1]);
		if (_topColor != color)
		{
			_topColor = color;
			_materialColorer.SetLightingColor(this, color);
		}
	}
}
