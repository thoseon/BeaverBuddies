using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.LifeSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Beavers;

internal class BeaverLongevity : BaseComponent, IAwakableComponent, ILongevity, IPersistentEntity
{
	private static readonly ComponentKey BeaverLongevityKey = new ComponentKey("BeaverLongevity");

	private static readonly PropertyKey<float> ExpectedLongevityKey = new PropertyKey<float>("ExpectedLongevity");

	private static readonly float MinExpectedLongevity = 0.9f;

	private static readonly float MaxExpectedLongevity = 1.1f;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	public float ExpectedLongevity { get; private set; }

	public BeaverLongevity(IRandomNumberGenerator randomNumberGenerator)
	{
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		ExpectedLongevity = _randomNumberGenerator.Range(MinExpectedLongevity, MaxExpectedLongevity);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(BeaverLongevityKey).Set(ExpectedLongevityKey, ExpectedLongevity);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(BeaverLongevityKey);
		ExpectedLongevity = component.Get(ExpectedLongevityKey);
	}
}
