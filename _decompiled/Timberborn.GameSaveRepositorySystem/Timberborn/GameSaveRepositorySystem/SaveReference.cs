namespace Timberborn.GameSaveRepositorySystem;

public record SaveReference(string SaveName, SettlementReference SettlementReference)
{
	public override string ToString()
	{
		return SettlementReference.SettlementName + " - " + SaveName;
	}
}
