using System.Collections.Immutable;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.MortalComponents;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Healthcare;

internal class BeaverInjuryTextureSetter : BaseComponent, IAwakableComponent, IDeadNeededComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly string InjuryNeedId = "Injury";

	private static readonly string InjuryDiffusePropertyName = "_InjuryDiffuse";

	private static readonly string InjuryNormalMapPropertyName = "_InjuryNormalMap";

	private static readonly string InjuryDisplacementPropertyName = "_InjuryDisplacement";

	private static readonly ComponentKey BeaverInjuryTextureSetterKey = new ComponentKey("BeaverInjuryTextureSetter");

	private static readonly PropertyKey<int> InjurySetIdKey = new PropertyKey<int>("InjurySetId");

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IAssetLoader _assetLoader;

	private CharacterMaterialModifier _characterMaterialModifier;

	private BeaverInjuryTextureSetterSpec _beaverInjuryTextureSetterSpec;

	private int _injuryDiffusePropertyName;

	private int _injuryNormalMapPropertyName;

	private int _injuryDisplacementPropertyName;

	private int _injurySetId;

	private ImmutableArray<BeaverInjuryTextureSet> InjuryTextureSets => _beaverInjuryTextureSetterSpec.InjuryTextureSets;

	public BeaverInjuryTextureSetter(IRandomNumberGenerator randomNumberGenerator, IAssetLoader assetLoader)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_assetLoader = assetLoader;
	}

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
		_beaverInjuryTextureSetterSpec = GetComponent<BeaverInjuryTextureSetterSpec>();
		_injuryDiffusePropertyName = Shader.PropertyToID(InjuryDiffusePropertyName);
		_injuryNormalMapPropertyName = Shader.PropertyToID(InjuryNormalMapPropertyName);
		_injuryDisplacementPropertyName = Shader.PropertyToID(InjuryDisplacementPropertyName);
		GetComponent<NeedManager>().NeedChangedActiveState += OnNeedChangedActiveState;
	}

	public void InitializeEntity()
	{
		bool isInjured = GetComponent<NeedManager>().NeedIsActive(InjuryNeedId);
		UpdateInjuryTextures(isInjured);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_injurySetId != 0)
		{
			entitySaver.GetComponent(BeaverInjuryTextureSetterKey).Set(InjurySetIdKey, _injurySetId);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BeaverInjuryTextureSetterKey, out var objectLoader) && objectLoader.Has(InjurySetIdKey))
		{
			_injurySetId = objectLoader.Get(InjurySetIdKey);
		}
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		if (e.NeedSpec.Id == InjuryNeedId)
		{
			if (e.IsActive)
			{
				RandomizeTextureSetId();
			}
			UpdateInjuryTextures(e.IsActive);
		}
	}

	private void RandomizeTextureSetId()
	{
		_injurySetId = _randomNumberGenerator.Range(0, InjuryTextureSets.Length);
	}

	private void UpdateInjuryTextures(bool isInjured)
	{
		if (isInjured)
		{
			SetInjuryTexturesInMaterial();
		}
		else
		{
			ClearInjuryTexturesInMaterial();
		}
	}

	private void SetInjuryTexturesInMaterial()
	{
		SetTexturesInMaterial(LoadTexture(InjuryTextureSets[_injurySetId].DiffusePath), LoadTexture(InjuryTextureSets[_injurySetId].NormalMapPath), LoadTexture(InjuryTextureSets[_injurySetId].DisplacementPath));
	}

	private Texture2D LoadTexture(string path)
	{
		return _assetLoader.Load<Texture2D>(path);
	}

	private void ClearInjuryTexturesInMaterial()
	{
		SetTexturesInMaterial(Texture2D.blackTexture, Texture2D.blackTexture, Texture2D.blackTexture);
	}

	private void SetTexturesInMaterial(Texture diffuse, Texture normalMap, Texture displacement)
	{
		_characterMaterialModifier.SetTexture(_injuryDiffusePropertyName, diffuse);
		_characterMaterialModifier.SetTexture(_injuryNormalMapPropertyName, normalMap);
		_characterMaterialModifier.SetTexture(_injuryDisplacementPropertyName, displacement);
	}
}
