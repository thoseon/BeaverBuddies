using System.Globalization;
using System.Text;
using Timberborn.Persistence;

namespace Timberborn.PackedListSystem;

public class FloatPackedListSerializer : PackedListSerializer<float>
{
	protected override void Serialize(float value, StringBuilder stringBuilder)
	{
		stringBuilder.Append(CommonNumberSerializer.SerializeFloat(value));
	}

	protected override float Deserialize(string value)
	{
		return float.Parse(value, CultureInfo.InvariantCulture);
	}
}
