using Bindito.Core;
using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

[Context("Bootstrapper")]
internal class ModdingAssetsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ModAssetBundleLoader>().AsSingleton();
		Bind<ModTextureSettingLoader>().AsSingleton();
		Bind<IModFileConverter<Sprite>>().To<ModSpriteConverter>().AsSingleton();
		Bind<IModFileConverter<Texture2D>>().To<ModTextureConverter>().AsSingleton();
		Bind<IModFileConverter<TextAsset>>().To<ModTextAssetConverter>().AsSingleton();
		Bind<IModFileConverter<BinaryData>>().To<ModTimbermeshConverter>().AsSingleton();
		Bind<IModFileConverter<BlueprintAsset>>().To<ModBlueprintConverter>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModSystemFileProvider<Sprite>>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModSystemFileProvider<Texture2D>>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModSystemFileProvider<TextAsset>>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModSystemFileProvider<BinaryData>>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModSystemFileProvider<BlueprintAsset>>().AsSingleton();
		MultiBind<IAssetProvider>().To<ModAssetBundleProvider>().AsSingleton();
	}
}
