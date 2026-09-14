using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Illumination;

public class IlluminationService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private float _iconExponent;

	private float _iconMultiplier;

	private FrozenDictionary<string, Color> _colorsById;

	public Color DefaultColor { get; private set; }

	public ImmutableArray<Color> PresetColors { get; private set; }

	public IlluminationService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		IlluminationServiceSpec singleSpec = _specService.GetSingleSpec<IlluminationServiceSpec>();
		_iconExponent = singleSpec.IconExponent;
		_iconMultiplier = singleSpec.IconMultiplier;
		ImmutableArray<IlluminationColorSpec> immutableArray = LoadUniqueColors();
		_colorsById = immutableArray.ToFrozenDictionary((IlluminationColorSpec spec) => spec.Id, (IlluminationColorSpec spec) => spec.Color);
		PresetColors = (from spec in immutableArray
			select spec.GetSpec<IlluminationPresetSpec>() into spec
			where spec != null
			orderby spec.Order
			select spec.GetSpec<IlluminationColorSpec>().Color).ToImmutableArray();
		DefaultColor = FindColorById(singleSpec.DefaultColorId);
	}

	public Color FindColorById(string id)
	{
		if (_colorsById.TryGetValue(id, out var value))
		{
			return value;
		}
		Debug.LogWarning("IlluminationColorSpec with id '" + id + "' does not exist!");
		return Color.white;
	}

	public Color LightingColorToIconColor(Color lightingColor)
	{
		Color.RGBToHSV(lightingColor, out var H, out var S, out var _);
		return Color.HSVToRGB(H, Mathf.Pow(S, _iconExponent) * _iconMultiplier, 1f);
	}

	private ImmutableArray<IlluminationColorSpec> LoadUniqueColors()
	{
		Dictionary<string, IlluminationColorSpec> dictionary = new Dictionary<string, IlluminationColorSpec>();
		foreach (IlluminationColorSpec spec in _specService.GetSpecs<IlluminationColorSpec>())
		{
			if (dictionary.TryGetValue(spec.Id, out var value))
			{
				Debug.LogWarning("IlluminationColorSpec with id '" + spec.Id + "' already exists." + $" Replacing {value.Color} with {spec.Color}.");
			}
			dictionary[spec.Id] = spec;
		}
		return dictionary.Values.ToImmutableArray();
	}
}
