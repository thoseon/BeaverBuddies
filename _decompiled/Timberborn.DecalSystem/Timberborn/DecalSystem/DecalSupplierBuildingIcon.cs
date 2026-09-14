using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.DecalSystem;

internal class DecalSupplierBuildingIcon : BaseComponent, IAwakableComponent, IInitializablePreview, IInitializableEntity, IDeletableEntity
{
	private static readonly int IconPropertyId = Shader.PropertyToID("_DetailAlbedoMap3");

	private readonly IDecalService _decalService;

	private DecalSupplier _decalSupplier;

	private MeshRenderer _iconRenderer;

	public DecalSupplierBuildingIcon(IDecalService decalService)
	{
		_decalService = decalService;
	}

	public void Awake()
	{
		_decalSupplier = GetComponent<DecalSupplier>();
		string iconRendererName = GetComponent<DecalSupplierBuildingIconSpec>().IconRendererName;
		_iconRenderer = base.GameObject.FindChild(iconRendererName).GetComponent<MeshRenderer>();
	}

	public void InitializePreview()
	{
		UpdateIcon();
	}

	public void InitializeEntity()
	{
		_decalSupplier.ActiveDecalChanged += delegate
		{
			UpdateIcon();
		};
		UpdateIcon();
	}

	public void DeleteEntity()
	{
		Object.Destroy(_iconRenderer.material);
	}

	private void UpdateIcon()
	{
		Decal validatedDecal = _decalService.GetValidatedDecal(_decalSupplier.ActiveDecal);
		Texture2D decalTexture = _decalService.GetDecalTexture(validatedDecal);
		_iconRenderer.material.SetTexture(IconPropertyId, decalTexture);
	}
}
