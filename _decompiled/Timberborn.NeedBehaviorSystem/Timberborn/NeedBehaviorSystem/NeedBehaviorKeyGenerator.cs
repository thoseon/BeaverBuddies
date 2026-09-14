using System.Collections.Generic;
using System.Text;
using Timberborn.NeedSpecs;

namespace Timberborn.NeedBehaviorSystem;

public class NeedBehaviorKeyGenerator
{
	private readonly List<string> _keyParts = new List<string>();

	private readonly StringBuilder _keyBuilder = new StringBuilder();

	public string GenerateKey(IReadOnlyList<InstantEffectSpec> effects)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			_keyParts.Add(effects[i].NeedId);
		}
		return GenerateKey("Instant");
	}

	public string GenerateKey(IReadOnlyList<ContinuousEffectSpec> effects)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			_keyParts.Add(effects[i].NeedId);
		}
		return GenerateKey("Continuous");
	}

	private string GenerateKey(string suffix)
	{
		_keyParts.Sort();
		for (int i = 0; i < _keyParts.Count; i++)
		{
			_keyBuilder.Append(_keyParts[i]);
		}
		_keyParts.Clear();
		_keyBuilder.Append(suffix);
		string result = _keyBuilder.ToString();
		_keyBuilder.Clear();
		return result;
	}
}
