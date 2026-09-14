using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Buildings;

public class BuildingSoundController : IPersistentEntity
{
	private static readonly ComponentKey BuildingSoundControllerKey = new ComponentKey("BuildingSoundController");

	private static readonly PropertyKey<bool> PlaySoundKey = new PropertyKey<bool>("PlaySound");

	public bool PlaySound { get; private set; } = true;

	public void Save(IEntitySaver entitySaver)
	{
		if (!PlaySound)
		{
			entitySaver.GetComponent(BuildingSoundControllerKey).Set(PlaySoundKey, PlaySound);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BuildingSoundControllerKey, out var objectLoader))
		{
			PlaySound = objectLoader.Get(PlaySoundKey);
		}
	}

	public void EnableSound()
	{
		PlaySound = true;
	}

	public void DisableSound()
	{
		PlaySound = false;
	}
}
