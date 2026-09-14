using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TransformControl;
using UnityEngine;

namespace Timberborn.NaturalResourcesModelSystem;

public class NaturalResourceModelRandomizer : BaseComponent, IAwakableComponent, IPreInitializableEntity
{
	private static readonly List<float> RotationsBy90Degree = new List<float> { 0f, 90f, 180f, 270f };

	private readonly IFakeRandomNumberGeneratorFactory _fakeRandomNumberGeneratorFactory;

	private IFakeRandomNumberGenerator _fakeRandomNumberGenerator;

	private NaturalResourceModelRandomizerSpec _naturalResourceModelRandomizerSpec;

	private float _heightScale;

	private float _rotation;

	public float DiameterScale { get; private set; }

	public NaturalResourceModelRandomizer(IFakeRandomNumberGeneratorFactory fakeRandomNumberGeneratorFactory)
	{
		_fakeRandomNumberGeneratorFactory = fakeRandomNumberGeneratorFactory;
	}

	public void Awake()
	{
		_naturalResourceModelRandomizerSpec = GetComponent<NaturalResourceModelRandomizerSpec>();
	}

	public void PreInitializeEntity()
	{
		_fakeRandomNumberGenerator = _fakeRandomNumberGeneratorFactory.Create(GetComponent<EntityComponent>().EntityId, 972389643);
		Randomize();
		Apply();
	}

	private void Randomize()
	{
		RandomizeHeightScale();
		RandomizeDiameterScale();
		RandomizeRotationAngle();
	}

	private void RandomizeHeightScale()
	{
		_heightScale = _fakeRandomNumberGenerator.Range(_naturalResourceModelRandomizerSpec.MinHeightScaleFactor, _naturalResourceModelRandomizerSpec.MaxHeightScaleFactor, 0);
	}

	private void RandomizeDiameterScale()
	{
		DiameterScale = (_naturalResourceModelRandomizerSpec.ConstrainProportion ? _heightScale : _fakeRandomNumberGenerator.Range(_naturalResourceModelRandomizerSpec.MinWidthScaleFactor, _naturalResourceModelRandomizerSpec.MaxWidthScaleFactor, 1));
	}

	private void RandomizeRotationAngle()
	{
		_rotation = _naturalResourceModelRandomizerSpec.RandomizedRotation switch
		{
			RandomizeRotationMode.By90Degree => RotationsBy90Degree[_fakeRandomNumberGenerator.Byte(2) % RotationsBy90Degree.Count], 
			RandomizeRotationMode.BetweenMinAndMax => _fakeRandomNumberGenerator.Range(_naturalResourceModelRandomizerSpec.MinRotation, _naturalResourceModelRandomizerSpec.MaxRotation, 2), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private void Apply()
	{
		TransformController component = GetComponent<TransformController>();
		component.AddScaleModifier().Set(new Vector3(DiameterScale, _heightScale, DiameterScale));
		component.AddRotationModifier(10).Set(Quaternion.AngleAxis(_rotation, Vector3.up));
	}
}
