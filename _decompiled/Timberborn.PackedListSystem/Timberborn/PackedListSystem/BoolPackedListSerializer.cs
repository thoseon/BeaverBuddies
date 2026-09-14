using System.Globalization;
using System.Text;

namespace Timberborn.PackedListSystem;

public class BoolPackedListSerializer : PackedListSerializer<bool>
{
	protected override void Serialize(bool value, StringBuilder stringBuilder)
	{
		stringBuilder.Append(value ? "1" : "0");
	}

	protected override bool Deserialize(string value)
	{
		return int.Parse(value, CultureInfo.InvariantCulture) > 0;
	}
}
