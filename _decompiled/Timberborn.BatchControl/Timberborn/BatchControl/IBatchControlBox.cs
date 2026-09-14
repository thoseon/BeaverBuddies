namespace Timberborn.BatchControl;

public interface IBatchControlBox
{
	void OpenBatchControlBox();

	void OpenCharactersTab();

	void OpenHousingTab();

	void OpenWorkplacesTab();

	void OpenMigrationTab();

	void OpenDistributionTab();

	void OpenTab(int index);
}
