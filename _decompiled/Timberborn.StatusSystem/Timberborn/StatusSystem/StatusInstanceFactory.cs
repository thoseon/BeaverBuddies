using System;
using Timberborn.SpriteOperations;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusInstanceFactory
{
	private static readonly int DefaultUISpriteSize = 32;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly SpriteResizer _spriteResizer;

	private readonly StatusSpriteLoader _statusSpriteLoader;

	public StatusInstanceFactory(IDayNightCycle dayNightCycle, SpriteResizer spriteResizer, StatusSpriteLoader statusSpriteLoader)
	{
		_dayNightCycle = dayNightCycle;
		_spriteResizer = spriteResizer;
		_statusSpriteLoader = statusSpriteLoader;
	}

	public StatusInstance CreateStatus(StatusSubject statusSubject, StatusToggle statusToggle)
	{
		return CreateStatusInternal(statusSubject, statusToggle, null, null, null);
	}

	public StatusInstance CreateDynamicStatus(StatusSubject statusSubject, StatusToggle statusToggle, Func<float> statusValueGetter, Func<StatusWarningType> statusWarningTypeGetter, string warningSound)
	{
		return CreateStatusInternal(statusSubject, statusToggle, statusValueGetter, statusWarningTypeGetter, warningSound);
	}

	private StatusInstance CreateStatusInternal(StatusSubject statusSubject, StatusToggle statusToggle, Func<float> statusValueGetter, Func<StatusWarningType> statusWarningTypeGetter, string warningSound)
	{
		StatusSpecification statusSpecification = statusToggle.StatusSpecification;
		Sprite sprite = _statusSpriteLoader.LoadSprite(statusSpecification.SpriteName);
		Sprite resizedSprite = _spriteResizer.GetResizedSprite(sprite, DefaultUISpriteSize);
		return new StatusInstance(statusSpecification.StatusDescription, statusSpecification.AlertDescription, statusToggle.IsPriorityStatus, statusToggle.IsNotifying, statusSpecification.ShowFloatingIcon, statusSubject, sprite, resizedSprite, statusValueGetter, statusWarningTypeGetter, warningSound, _dayNightCycle, statusSpecification.DelayInHours);
	}
}
