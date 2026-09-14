namespace Timberborn.Modding;

public class Mod
{
	private bool _isIdDuplicated;

	public ModDirectory ModDirectory { get; }

	public ModManifest Manifest { get; }

	public bool IsEnabled { get; }

	public string DisplayName
	{
		get
		{
			if (!_isIdDuplicated)
			{
				return Manifest.Name;
			}
			return ModDirectory.OriginName + "/" + Manifest.Name;
		}
	}

	public Mod(ModDirectory modDirectory, ModManifest manifest, bool isEnabled)
	{
		ModDirectory = modDirectory;
		Manifest = manifest;
		IsEnabled = isEnabled;
	}

	public void MarkIdAsDuplicated()
	{
		_isIdDuplicated = true;
	}
}
