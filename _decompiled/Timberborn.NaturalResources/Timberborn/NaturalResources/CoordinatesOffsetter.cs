using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TransformControl;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.NaturalResources;

public class CoordinatesOffsetter : BaseComponent, IPreInitializableEntity, IPersistentEntity
{
	private static readonly float MaxCoordinateOffset = 0.5f;

	private static readonly ComponentKey CoordinatesOffsetterKey = new ComponentKey("CoordinatesOffsetter");

	private static readonly PropertyKey<bool> RandomKey = new PropertyKey<bool>("Random");

	private readonly IFakeRandomNumberGeneratorFactory _fakeRandomNumberGeneratorFactory;

	private IFakeRandomNumberGenerator _fakeRandomNumberGenerator;

	public Vector2 CoordinatesOffset { get; private set; } = Vector2.zero;

	public CoordinatesOffsetter(IFakeRandomNumberGeneratorFactory fakeRandomNumberGeneratorFactory)
	{
		_fakeRandomNumberGeneratorFactory = fakeRandomNumberGeneratorFactory;
	}

	public void PreInitializeEntity()
	{
		if (HasComponent<CoordinatesOffsetterInit>())
		{
			SetRandomOffset();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (CoordinatesOffset != Vector2.zero)
		{
			entitySaver.GetComponent(CoordinatesOffsetterKey).Set(RandomKey, value: true);
		}
	}

	[BackwardCompatible(2025, 4, 20, Compatibility.Map)]
	public void Load(IEntityLoader entityLoader)
	{
		ComponentKey key = new ComponentKey("CoordinatesOffseter");
		PropertyKey<Vector2> key2 = new PropertyKey<Vector2>("CoordinatesOffset");
		IObjectLoader objectLoader2;
		if (entityLoader.TryGetComponent(CoordinatesOffsetterKey, out var objectLoader))
		{
			if (objectLoader.Has(RandomKey) && objectLoader.Get(RandomKey))
			{
				SetRandomOffset();
			}
		}
		else if (entityLoader.TryGetComponent(key, out objectLoader2) && objectLoader2.Has(key2) && objectLoader2.Get(key2) != Vector2.zero)
		{
			SetRandomOffset();
		}
	}

	private void SetRandomOffset()
	{
		_fakeRandomNumberGenerator = _fakeRandomNumberGeneratorFactory.Create(GetComponent<EntityComponent>().EntityId, 208621589);
		CoordinatesOffset = new Vector2(RandomCoordinateOffset(3), RandomCoordinateOffset(0));
		GetComponent<TransformController>().AddPositionModifier().Set(CoordinateSystem.GridToWorld(CoordinatesOffset));
	}

	private float RandomCoordinateOffset(int byteIndex)
	{
		return _fakeRandomNumberGenerator.Range(-0.5f, 0.5f, byteIndex) * MaxCoordinateOffset;
	}
}
