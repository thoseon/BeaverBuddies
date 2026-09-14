using System.Text;

namespace Timberborn.Common;

public struct StringListBuilder(StringBuilder stringBuilder, string separator)
{
	private readonly StringBuilder _stringBuilder = stringBuilder;

	private readonly string _separator = separator;

	private bool _subsequent = false;

	public void BeginItem()
	{
		if (_subsequent)
		{
			_stringBuilder.Append(_separator);
		}
		else
		{
			_subsequent = true;
		}
	}
}
