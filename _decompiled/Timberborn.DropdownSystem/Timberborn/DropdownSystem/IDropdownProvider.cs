using System.Collections.Generic;

namespace Timberborn.DropdownSystem;

public interface IDropdownProvider
{
	IReadOnlyList<string> Items { get; }

	string GetValue();

	void SetValue(string value);
}
