using System.Globalization;
using System.Text;
using Timberborn.Common;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;

namespace Timberborn.WaterSystem;

internal class WaterColumnPackedListSerializer : PackedListSerializer<WaterColumn>
{
	private static readonly char Separator = ':';

	private static readonly string EmptyColumnValue = "0";

	protected override void Serialize(WaterColumn value, StringBuilder stringBuilder)
	{
		byte floor = value.Floor;
		float waterDepth = value.WaterDepth;
		float contamination = value.Contamination;
		float overflow = value.Overflow;
		float oldWaterDepth = value.OldWaterDepth;
		if (waterDepth == 0f && contamination == 0f && overflow == 0f)
		{
			stringBuilder.Append(EmptyColumnValue);
			return;
		}
		stringBuilder.Append(waterDepth.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeFloat(contamination));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeFloat(overflow));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(floor));
		stringBuilder.Append(Separator);
		stringBuilder.Append(CommonNumberSerializer.SerializeFloat(oldWaterDepth));
	}

	[BackwardCompatible(2026, 4, 29, Compatibility.Map)]
	protected override WaterColumn Deserialize(string value)
	{
		if (value == EmptyColumnValue)
		{
			return default(WaterColumn);
		}
		string[] array = value.Split(Separator);
		float num = float.Parse(array[0]);
		return new WaterColumn
		{
			WaterDepth = num,
			Contamination = float.Parse(array[1]),
			Overflow = float.Parse(array[2]),
			Floor = (byte)((array.Length >= 4) ? byte.Parse(array[3]) : 0),
			OldWaterDepth = ((array.Length >= 5) ? float.Parse(array[4]) : num)
		};
	}
}
