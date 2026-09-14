using Timberborn.Common;
using Timberborn.Debugging;
using Timberborn.Modding;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameScene;

internal class DateSalter : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey DateSalterKey = new SingletonKey("DateSaltService");

	private static readonly PropertyKey<int> DateSaltedIdKey = new PropertyKey<int>("DateSaltedId");

	private static readonly PropertyKey<int> TimeSaltedIdKey = new PropertyKey<int>("TimeSaltedId");

	private readonly EventBus _eventBus;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly DevModeManager _devModeManager;

	private readonly ISingletonLoader _singletonLoader;

	private bool _saltDateIdWithOddNumber;

	private bool _saltTimeIdWithOddNumber;

	public DateSalter(EventBus eventBus, IRandomNumberGenerator randomNumberGenerator, DevModeManager devModeManager, ISingletonLoader singletonLoader)
	{
		_eventBus = eventBus;
		_randomNumberGenerator = randomNumberGenerator;
		_devModeManager = devModeManager;
		_singletonLoader = singletonLoader;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(DateSalterKey, out var objectLoader))
		{
			_saltDateIdWithOddNumber = IsOddNumber(objectLoader.Get(DateSaltedIdKey));
			_saltTimeIdWithOddNumber = IsOddNumber(objectLoader.Get(TimeSaltedIdKey));
		}
		if (_devModeManager.Enabled)
		{
			_saltDateIdWithOddNumber = true;
		}
		if (ModdedState.IsModded)
		{
			_saltTimeIdWithOddNumber = true;
		}
		_eventBus.Register(this);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(DateSalterKey);
		singleton.Set(DateSaltedIdKey, GenerateRandomNumber(_saltDateIdWithOddNumber));
		singleton.Set(TimeSaltedIdKey, GenerateRandomNumber(_saltTimeIdWithOddNumber));
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		if (devModeToggledEvent.Enabled)
		{
			_saltDateIdWithOddNumber = true;
		}
	}

	private int GenerateRandomNumber(bool odd)
	{
		int num = _randomNumberGenerator.Range(0, 1000000);
		if (odd != IsOddNumber(num))
		{
			num++;
		}
		return num;
	}

	private static bool IsOddNumber(int number)
	{
		return number % 2 == 1;
	}
}
