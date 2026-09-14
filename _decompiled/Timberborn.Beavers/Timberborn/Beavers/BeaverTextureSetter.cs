using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.FactionSystem;
using Timberborn.GameFactionSystem;
using Timberborn.MortalComponents;
using UnityEngine;

namespace Timberborn.Beavers;

public class BeaverTextureSetter : BaseComponent, IInitializableEntity, IDeadNeededComponent
{
	private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

	private readonly FactionService _factionService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	public BeaverTextureSetter(FactionService factionService, IRandomNumberGenerator randomNumberGenerator)
	{
		_factionService = factionService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void InitializeEntity()
	{
		CharacterMaterialModifier component = GetComponent<CharacterMaterialModifier>();
		Child component2 = GetComponent<Child>();
		FactionSpec current = _factionService.Current;
		ImmutableArray<AssetRef<Texture2D>> immutableArray = (component2 ? current.ChildTextures : current.Textures);
		component.SetTexture(texture: _randomNumberGenerator.GetEnumerableElement(immutableArray).Asset, propertyId: BaseMapId);
	}
}
