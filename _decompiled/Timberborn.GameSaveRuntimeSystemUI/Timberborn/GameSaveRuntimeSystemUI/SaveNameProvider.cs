using System.Linq;
using Timberborn.Common;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.SettlementNameSystem;

namespace Timberborn.GameSaveRuntimeSystemUI;

public class SaveNameProvider
{
	private readonly SettlementReferenceService _settlementReferenceService;

	public SaveNameProvider(SettlementReferenceService settlementReferenceService)
	{
		_settlementReferenceService = settlementReferenceService;
	}

	public string GetDefaultSaveName(ReadOnlyList<GameSaveItem> existingSaves)
	{
		string settlementName = _settlementReferenceService.SettlementReference.SettlementName;
		int num = 0;
		string saveName = settlementName;
		while (existingSaves.Any((GameSaveItem s) => s.SaveReference.SaveName == saveName))
		{
			saveName = $"{settlementName} ({++num})";
		}
		return saveName;
	}
}
