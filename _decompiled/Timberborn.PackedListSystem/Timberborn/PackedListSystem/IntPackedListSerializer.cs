using System.Globalization;
using System.Text;
using Timberborn.Persistence;

namespace Timberborn.PackedListSystem;

public class IntPackedListSerializer : PackedListSerializer<int>
{
	protected override void Serialize(int value, StringBuilder stringBuilder)
	{
		stringBuilder.Append(CommonNumberSerializer.SerializeInt(value));
	}

	protected override int Deserialize(string value)
	{
		return int.Parse(value, CultureInfo.InvariantCulture);
	}
}
