using System.Text;
using Timberborn.Common;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;

namespace Timberborn.PopulationStatisticsSampling;

public class PopulationSamplePackedListSerializer : PackedListSerializer<PopulationSample>
{
	private static readonly char Separator = ':';

	protected override void Serialize(PopulationSample value, StringBuilder stringBuilder)
	{
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Day));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Cycle));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Adults));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Children));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Contaminated));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Bots));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Wellbeing));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Births));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Deaths));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.BotCreations));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.BotDestructions));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.OccupiedBeds));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.FreeBeds));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.Homeless));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.EmployedBeavers));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.FreeWorkSlotsBeavers));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.UnemployedBeavers));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.UnemployableBeavers));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.EmployedBots));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.FreeWorkSlotsBots));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.UnemployedBots));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value.UnemployableBots));
	}

	[BackwardCompatible(2026, 5, 13, Compatibility.Save)]
	protected override PopulationSample Deserialize(string value)
	{
		string[] array = value.Split(Separator);
		if (array.Length != 22)
		{
			return default(PopulationSample);
		}
		return new PopulationSample(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]), int.Parse(array[6]), int.Parse(array[7]), int.Parse(array[8]), int.Parse(array[9]), int.Parse(array[10]), int.Parse(array[11]), int.Parse(array[12]), int.Parse(array[13]), int.Parse(array[14]), int.Parse(array[15]), int.Parse(array[16]), int.Parse(array[17]), int.Parse(array[18]), int.Parse(array[19]), int.Parse(array[20]), int.Parse(array[21]));
	}
}
