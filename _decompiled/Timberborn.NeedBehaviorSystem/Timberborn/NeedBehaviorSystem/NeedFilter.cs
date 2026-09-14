using Timberborn.NeedSystem;

namespace Timberborn.NeedBehaviorSystem;

public readonly struct NeedFilter
{
	private readonly NeedManager _needManager;

	private readonly bool _onlyCritical;

	private readonly bool _onlyCriticalState;

	private readonly bool _belowWarningThreshold;

	public bool OnlyCriticalStateNeeds
	{
		get
		{
			if (_needManager != null)
			{
				return _onlyCriticalState;
			}
			return false;
		}
	}

	private NeedFilter(NeedManager needManager, bool onlyCritical, bool onlyCriticalState, bool belowWarningThreshold)
	{
		_needManager = needManager;
		_onlyCritical = onlyCritical;
		_onlyCriticalState = onlyCriticalState;
		_belowWarningThreshold = belowWarningThreshold;
	}

	public static NeedFilter NeedIsInCriticalState(NeedManager needManager)
	{
		return new NeedFilter(needManager, onlyCritical: true, onlyCriticalState: true, belowWarningThreshold: false);
	}

	public static NeedFilter NeedIsCritical(NeedManager needManager)
	{
		return new NeedFilter(needManager, onlyCritical: true, onlyCriticalState: false, belowWarningThreshold: false);
	}

	public static NeedFilter NeedIsBelowWarningThreshold(NeedManager needManager)
	{
		return new NeedFilter(needManager, onlyCritical: false, onlyCriticalState: false, belowWarningThreshold: true);
	}

	public static NeedFilter AnyNeed()
	{
		return new NeedFilter(null, onlyCritical: false, onlyCriticalState: false, belowWarningThreshold: false);
	}

	public bool Filter(string needId)
	{
		if (_needManager == null)
		{
			return true;
		}
		if (_onlyCritical && !_needManager.NeedIsCritical(needId))
		{
			return false;
		}
		if (_onlyCriticalState && !_needManager.NeedIsInCriticalState(needId))
		{
			return false;
		}
		if (_belowWarningThreshold)
		{
			return _needManager.NeedIsBelowWarningThreshold(needId);
		}
		return true;
	}
}
