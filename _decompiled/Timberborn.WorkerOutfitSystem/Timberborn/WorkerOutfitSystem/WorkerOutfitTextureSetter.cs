using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using UnityEngine;

namespace Timberborn.WorkerOutfitSystem;

internal class WorkerOutfitTextureSetter : BaseComponent, IAwakableComponent
{
	private static readonly int DiffuseTextureId = Shader.PropertyToID("_WorkerOutfitDiffuse");

	private static readonly int NormalTextureId = Shader.PropertyToID("_WorkerOutfitNormal");

	private CharacterMaterialModifier _characterMaterialModifier;

	private bool _diffuseTextureSet;

	private bool _normalTextureSet;

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
		GetComponent<WorkerOutfitChangeNotifier>().OutfitChanged += OnOutfitChanged;
	}

	private void OnOutfitChanged(object sender, WorkerOutfitChangedEventArgs e)
	{
		SetTexture(e.WorkerOutfitSpec?.DiffuseTexture?.Asset, DiffuseTextureId, ref _diffuseTextureSet);
		SetTexture(e.WorkerOutfitSpec?.NormalTexture?.Asset, NormalTextureId, ref _normalTextureSet);
	}

	private void SetTexture(Texture texture, int textureId, ref bool setFlag)
	{
		if ((bool)texture)
		{
			_characterMaterialModifier.SetTexture(textureId, texture);
			setFlag = true;
		}
		else if (setFlag)
		{
			_characterMaterialModifier.SetTexture(textureId, null);
			setFlag = false;
		}
	}
}
