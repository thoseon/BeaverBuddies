using System.IO;
using Newtonsoft.Json.Linq;
using Timberborn.AssetSystem;
using Timberborn.Common;
using Timberborn.FileSystem;
using Timberborn.Modding;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.MainMenuModdingUI;

public class ModCreator
{
	private static readonly string TemplatesPath = Path.Combine(Application.streamingAssetsPath, "Modding", "ModTemplates");

	private static readonly string LocalizationsDirectory = "Localizations";

	private static readonly string BaseLanguageName = "English (English)";

	private static readonly string BaseLanguageCode = "enUS";

	private static readonly string DoNotTranslatePostfix = "_donottranslate";

	private static readonly string ModPostfix = "_mod";

	private static readonly string LocalizationExtension = ".csv";

	private readonly IFileService _fileService;

	private readonly FilenameValidator _filenameValidator;

	private readonly ModTemplateDropdownProvider _modTemplateDropdownProvider;

	private readonly IAssetLoader _assetLoader;

	public ModCreator(IFileService fileService, FilenameValidator filenameValidator, ModTemplateDropdownProvider modTemplateDropdownProvider, IAssetLoader assetLoader)
	{
		_fileService = fileService;
		_filenameValidator = filenameValidator;
		_modTemplateDropdownProvider = modTemplateDropdownProvider;
		_assetLoader = assetLoader;
	}

	public DirectoryCreationResult CreateMod(string modName, string localizationCode, out string destinationPath)
	{
		if (_filenameValidator.NameIsInvalid(modName))
		{
			destinationPath = null;
			return DirectoryCreationResult.NameInvalid;
		}
		string sourcePath = Path.Combine(TemplatesPath, _modTemplateDropdownProvider.GetDirectory());
		destinationPath = Path.Combine(UserDataFolder.Folder, UserFolderModsProvider.ModsDirectoryName, modName);
		DirectoryCreationResult num = _fileService.CreateDirectoryIfValid(destinationPath);
		if (num == DirectoryCreationResult.OK)
		{
			CopyDirectory(sourcePath, destinationPath, modName);
			if (!string.IsNullOrEmpty(localizationCode))
			{
				CreateLocalizationFiles(modName, localizationCode, destinationPath);
			}
		}
		return num;
	}

	private void CopyDirectory(string sourcePath, string targetPath, string modName)
	{
		Directory.CreateDirectory(targetPath);
		FileInfo[] files = new DirectoryInfo(sourcePath).GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Extension != ".meta")
			{
				string text = Path.Combine(targetPath, fileInfo.Name);
				if (fileInfo.Name == ManifestLoader.ManifestFileName)
				{
					string manifestContent = GetManifestContent(File.ReadAllText(fileInfo.FullName), modName);
					_fileService.WriteTextToFile(text, manifestContent);
				}
				else
				{
					_fileService.CopyFile(fileInfo.FullName, text);
				}
			}
		}
		string[] directories = Directory.GetDirectories(sourcePath);
		foreach (string text2 in directories)
		{
			string fileName = Path.GetFileName(text2);
			string targetPath2 = Path.Combine(targetPath, fileName);
			CopyDirectory(text2, targetPath2, modName);
		}
	}

	private string GetManifestContent(string original, string modName)
	{
		string value = _modTemplateDropdownProvider.GetValue();
		JObject jObject = JObject.Parse(original);
		jObject["Name"] = modName;
		jObject["Id"] = value.ToPascalCase() + "." + modName.ToPascalCase();
		return jObject.ToString();
	}

	private void CreateLocalizationFiles(string modName, string localizationCode, string destinationPath)
	{
		string text = Path.Combine(destinationPath, LocalizationsDirectory);
		Directory.CreateDirectory(text);
		foreach (LoadedAsset<TextAsset> item in _assetLoader.LoadAll<TextAsset>(LocalizationsDirectory ?? ""))
		{
			if (item.Asset.name.StartsWith(BaseLanguageCode))
			{
				CreateLocalizationFile(text, item.Asset, modName, localizationCode, item.IsBuiltIn);
			}
		}
	}

	private void CreateLocalizationFile(string destinationPath, TextAsset original, string modName, string localizationCode, bool isBuiltIn)
	{
		string text = original.name.Replace(BaseLanguageCode, localizationCode);
		if (!isBuiltIn)
		{
			text += ModPostfix;
		}
		string path = Path.Combine(destinationPath, text + LocalizationExtension);
		int num = 1;
		while (File.Exists(path))
		{
			path = Path.Combine(destinationPath, $"{text}{num++}{LocalizationExtension}");
		}
		string text2 = (original.name.EndsWith(DoNotTranslatePostfix) ? original.text.Replace(BaseLanguageName, modName) : original.text);
		_fileService.WriteTextToFile(path, text2);
	}
}
