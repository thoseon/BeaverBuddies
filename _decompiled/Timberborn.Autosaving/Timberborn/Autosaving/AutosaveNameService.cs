using System;
using Timberborn.GameCycleSystem;
using Timberborn.GameSaveRepositorySystem;

namespace Timberborn.Autosaving;

public class AutosaveNameService
{
	private static readonly string NameSuffix = GameSaveRepository.AutosaveNameSuffix;

	private static readonly string GameDatePattern = "Day {0}-{1}";

	private readonly GameCycleService _gameCycleService;

	public AutosaveNameService(GameCycleService gameCycleService)
	{
		_gameCycleService = gameCycleService;
	}

	public string GetAutosaveName()
	{
		return Timestamp() + NameSuffix;
	}

	public bool IsAutosaveName(string name)
	{
		return name.EndsWith(NameSuffix);
	}

	private string Timestamp()
	{
		string text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH\\hmm\\m");
		string text2 = string.Format(GameDatePattern, _gameCycleService.Cycle, _gameCycleService.CycleDay);
		return text + ", " + text2;
	}
}
