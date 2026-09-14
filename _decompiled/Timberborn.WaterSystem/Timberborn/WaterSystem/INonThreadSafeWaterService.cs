namespace Timberborn.WaterSystem;

public interface INonThreadSafeWaterService
{
	void UpdateOutflowsData();

	ReadOnlyWaterColumn GetColumnByIndex(int index3D);

	ReadOnlyColumnOutflows ColumnOutflows(int index3D);

	int GetColumnCount(int index);
}
