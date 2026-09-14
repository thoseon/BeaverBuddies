using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.DecalSystem;

public class FlippableDecal : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<FlippableDecal>, IDuplicable
{
	private static readonly ComponentKey FlippableDecalKey = new ComponentKey("FlippableDecal");

	private static readonly PropertyKey<bool> IsFlippedKey = new PropertyKey<bool>("IsFlipped");

	private Transform _decalTransform;

	public bool IsFlipped { get; private set; }

	public void Awake()
	{
		FlippableDecalSpec component = GetComponent<FlippableDecalSpec>();
		_decalTransform = base.GameObject.FindChildTransform(component.DecalName);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(FlippableDecalKey).Set(IsFlippedKey, IsFlipped);
	}

	[BackwardCompatible(2025, 11, 7, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(FlippableDecalKey, out var objectLoader))
		{
			SetFlip(objectLoader.Get(IsFlippedKey));
		}
	}

	public void SetFlip(bool value)
	{
		IsFlipped = value;
		if ((IsFlipped && _decalTransform.localScale.x > 0f) || (!IsFlipped && _decalTransform.localScale.x < 0f))
		{
			FlipDecal();
		}
	}

	public void DuplicateFrom(FlippableDecal source)
	{
		SetFlip(source.IsFlipped);
	}

	private void FlipDecal()
	{
		Vector3 localScale = _decalTransform.localScale;
		localScale.x *= -1f;
		_decalTransform.localScale = localScale;
	}
}
