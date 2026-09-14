using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Timberborn.Modding;

namespace Timberborn.ModManagerScene;

public class ModCodeStarter
{
	private readonly ModRepository _modRepository;

	private readonly Dictionary<Mod, List<Assembly>> _loadedAssemblies = new Dictionary<Mod, List<Assembly>>();

	public ModCodeStarter(ModRepository modRepository)
	{
		_modRepository = modRepository;
	}

	public void Start()
	{
		LoadAssemblies();
		StartMods();
	}

	private void LoadAssemblies()
	{
		foreach (Mod enabledMod in _modRepository.EnabledMods)
		{
			LoadAssemblies(enabledMod);
		}
	}

	private void LoadAssemblies(Mod mod)
	{
		_loadedAssemblies[mod] = new List<Assembly>();
		FileInfo[] files = mod.ModDirectory.Directory.GetFiles("*.dll", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			Assembly item = Assembly.Load(File.ReadAllBytes(files[i].FullName));
			_loadedAssemblies[mod].Add(item);
		}
	}

	private void StartMods()
	{
		foreach (Mod enabledMod in _modRepository.EnabledMods)
		{
			StartMod(enabledMod);
		}
	}

	private void StartMod(Mod mod)
	{
		ModEnvironment modEnvironment = ModEnvironment.Create(mod);
		foreach (Type modStarter in GetModStarters(mod))
		{
			((IModStarter)Activator.CreateInstance(modStarter)).StartMod(modEnvironment);
		}
	}

	private IEnumerable<Type> GetModStarters(Mod mod)
	{
		Type modStarterType = typeof(IModStarter);
		return from type in _loadedAssemblies[mod].SelectMany((Assembly assembly) => assembly.GetTypes())
			where modStarterType.IsAssignableFrom(type) && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null
			select type;
	}
}
