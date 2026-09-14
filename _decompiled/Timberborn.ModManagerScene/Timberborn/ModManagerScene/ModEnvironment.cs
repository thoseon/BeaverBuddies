using Timberborn.Modding;

namespace Timberborn.ModManagerScene;

internal class ModEnvironment : IModEnvironment
{
	public string ModPath { get; }

	public string OriginPath { get; }

	private ModEnvironment(string modPath, string originPath)
	{
		ModPath = modPath;
		OriginPath = originPath;
	}

	public static ModEnvironment Create(Mod mod)
	{
		return new ModEnvironment(mod.ModDirectory.Directory.FullName, mod.ModDirectory.OriginPath);
	}
}
