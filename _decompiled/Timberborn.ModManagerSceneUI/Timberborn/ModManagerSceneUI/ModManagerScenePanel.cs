using System.Collections.Generic;
using System.Linq;
using Timberborn.CommandLine;
using Timberborn.FileSystem;
using Timberborn.ModManagerScene;
using Timberborn.Modding;
using Timberborn.ModdingAssets;
using Timberborn.ModdingUI;
using Timberborn.PlatformUtilities;
using Timberborn.SerializationSystem;
using Timberborn.SteamStoreSystem;
using Timberborn.SteamWorkshopContent;
using Timberborn.SteamWorkshopModDownloading;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Timberborn.ModManagerSceneUI;

internal class ModManagerScenePanel : MonoBehaviour
{
	private static readonly string MainMenuSceneName = "1-MainMenuScene";

	private static readonly string BenchmarkLengthKey = "benchmarkLength";

	private static readonly string SkipModManagerKey = "skipModManager";

	[SerializeField]
	private UIDocument _uiDocument;

	private readonly FileService _fileService = new FileService();

	private VisualElement _modManagerBox;

	private ModListView _modListView;

	private bool AutoStartingInEditor => false;

	public void Awake()
	{
		if (_fileService.HasDocumentsPermissions && !ShouldIgnoreMods())
		{
			ModRepository modRepository = CreateModRepository();
			if (modRepository != null && modRepository.Mods.Any())
			{
				if (AutoStartingInEditor || ShouldSkipModManager())
				{
					LoadModsAndStartGame();
				}
				else
				{
					InitializeModManagerPanel(modRepository.Mods);
				}
				return;
			}
		}
		StartGame();
	}

	public void Update()
	{
		if (WasKeyReleased(Key.Enter) || WasKeyReleased(Key.NumpadEnter))
		{
			LoadModsAndStartGame();
		}
		else
		{
			_modListView?.Update();
		}
	}

	private static bool ShouldIgnoreMods()
	{
		return CommandLineArguments.CreateWithCommandLineArgs().Has(BenchmarkLengthKey);
	}

	private static bool ShouldSkipModManager()
	{
		return CommandLineArguments.CreateWithCommandLineArgs().Has(SkipModManagerKey);
	}

	private ModRepository CreateModRepository()
	{
		ModLoader modLoader = new ModLoader(new ManifestLoader(new SerializedObjectReaderWriter(new JsonMerger())));
		ModRepository modRepository = new ModRepository(modLoader, new ModSorter(), GetModProviders(modLoader));
		modRepository.Load();
		return modRepository;
	}

	private IEnumerable<IModsProvider> GetModProviders(ModLoader modLoader)
	{
		yield return new UserFolderModsProvider(_fileService);
		SteamManager steamManager = new SteamManager();
		steamManager.Load();
		if (steamManager.Initialized)
		{
			yield return new SteamWorkshopModsProvider(new SteamWorkshopContentProvider(steamManager), modLoader);
		}
	}

	private void LoadModsAndStartGame()
	{
		ModRepository modRepository = CreateModRepository();
		List<Mod> list = modRepository.Mods.Where((Mod mod) => mod.IsEnabled).ToList();
		if (list.Any())
		{
			ModdedState.SetOfficialMods(list);
		}
		new ModCodeStarter(modRepository).Start();
		new ModAssetBundleLoader(modRepository).Load();
		StartGame();
	}

	private void StartGame()
	{
		if (_modManagerBox != null)
		{
			_modManagerBox.style.display = DisplayStyle.None;
		}
		SceneManager.LoadScene(MainMenuSceneName);
	}

	private void InitializeModManagerPanel(IEnumerable<Mod> mods)
	{
		_modListView = new ModListView(GetComponent<ModManagerSceneItemFactory>(), new ModSorter());
		VisualElement rootVisualElement = _uiDocument.rootVisualElement;
		_modListView.Initialize(rootVisualElement, mods);
		rootVisualElement.Q<ScrollView>().mouseWheelScrollSize = ScrollWheelSpeed.WheelScrollSize.Value;
		rootVisualElement.Q<Button>("StartButton").RegisterCallback<ClickEvent>(delegate
		{
			LoadModsAndStartGame();
		});
		_modManagerBox = rootVisualElement.Q<VisualElement>("ModManagerBox");
		rootVisualElement.Q<Label>("GameVersion").text = GameVersions.CurrentVersion.Formatted;
	}

	private static bool WasKeyReleased(Key key)
	{
		return Keyboard.current[key].wasReleasedThisFrame;
	}
}
