using Timberborn.Localization;
using Timberborn.Modding;
using Timberborn.SerializationSystem;
using Timberborn.SteamWorkshop;
using Timberborn.TextureOperations;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal class SteamWorkshopUploadableModFactory
{
	private readonly SteamWorkshopItemSerializer _steamWorkshopItemSerializer;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly TextureFactory _textureFactory;

	private readonly ILoc _loc;

	public SteamWorkshopUploadableModFactory(SteamWorkshopItemSerializer steamWorkshopItemSerializer, SerializedObjectReaderWriter serializedObjectReaderWriter, TextureFactory textureFactory, ILoc loc)
	{
		_steamWorkshopItemSerializer = steamWorkshopItemSerializer;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_textureFactory = textureFactory;
		_loc = loc;
	}

	public SteamWorkshopUploadableMod Create(Mod mod)
	{
		SteamWorkshopModDataFile steamWorkshopModDataFile = SteamWorkshopModDataFile.Create(_steamWorkshopItemSerializer, _serializedObjectReaderWriter, mod.ModDirectory.OriginPath);
		SteamWorkshopModThumbnail steamWorkshopModThumbnail = new SteamWorkshopModThumbnail(_textureFactory, mod.ModDirectory.OriginPath);
		steamWorkshopModThumbnail.UpdateThumbnail();
		return new SteamWorkshopUploadableMod(_loc, steamWorkshopModDataFile, mod, steamWorkshopModThumbnail);
	}
}
