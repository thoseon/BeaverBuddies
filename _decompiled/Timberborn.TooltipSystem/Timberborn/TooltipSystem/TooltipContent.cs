using System;
using UnityEngine.UIElements;

namespace Timberborn.TooltipSystem;

public readonly struct TooltipContent
{
	private static readonly float DefaultDelay = 0.3f;

	private readonly Func<string> _baseTextGetter;

	private readonly Func<string> _keyBindingGetter;

	private readonly Func<VisualElement> _visualElementGetter;

	private readonly bool _instant;

	public bool UpdatableContent { get; }

	public string BaseText => _baseTextGetter?.Invoke();

	public VisualElement VisualElement => _visualElementGetter?.Invoke();

	public float Delay
	{
		get
		{
			if (!_instant)
			{
				return DefaultDelay;
			}
			return 0f;
		}
	}

	private TooltipContent(Func<string> baseTextGetter, Func<VisualElement> visualElementGetter, bool instant, bool updatableContent = false, Func<string> keyBindingGetter = null)
	{
		_baseTextGetter = baseTextGetter;
		_visualElementGetter = visualElementGetter;
		_instant = instant;
		UpdatableContent = updatableContent;
		_keyBindingGetter = keyBindingGetter;
	}

	public static TooltipContent Create(Func<string> baseTextGetter)
	{
		return new TooltipContent(baseTextGetter, null, instant: false);
	}

	public static TooltipContent CreateWithKeyBinding(string baseText, Func<string> keyBindingGetter)
	{
		return new TooltipContent(() => baseText, null, instant: false, updatableContent: false, keyBindingGetter);
	}

	public static TooltipContent CreateWithKeyBinding(Func<string> baseTextGetter, Func<string> keyBindingGetter)
	{
		return new TooltipContent(baseTextGetter, null, instant: false, updatableContent: false, keyBindingGetter);
	}

	public static TooltipContent CreateUpdatable(Func<string> baseTextGetter)
	{
		return new TooltipContent(baseTextGetter, null, instant: false, updatableContent: true);
	}

	public static TooltipContent CreateInstantUpdatable(Func<VisualElement> visualElementGetter)
	{
		return new TooltipContent(null, visualElementGetter, instant: true, updatableContent: true);
	}

	public static TooltipContent Create(Func<VisualElement> visualElementGetter)
	{
		return new TooltipContent(null, visualElementGetter, instant: false);
	}

	public static TooltipContent CreateInstant(Func<VisualElement> visualElementGetter)
	{
		return new TooltipContent(null, visualElementGetter, instant: true);
	}

	public static TooltipContent CreateInstant(string baseText)
	{
		return new TooltipContent(() => baseText, null, instant: true);
	}

	public static TooltipContent CreateEmpty()
	{
		return new TooltipContent(null, null, instant: false);
	}

	public bool TryGetKeyBinding(out string keyBinding)
	{
		if (_keyBindingGetter != null)
		{
			keyBinding = _keyBindingGetter();
			return !string.IsNullOrWhiteSpace(keyBinding);
		}
		keyBinding = null;
		return false;
	}

	public bool HasContent()
	{
		if (string.IsNullOrWhiteSpace(BaseText))
		{
			return VisualElement != null;
		}
		return true;
	}
}
