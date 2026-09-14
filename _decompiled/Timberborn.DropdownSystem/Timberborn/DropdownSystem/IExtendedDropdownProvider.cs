using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.DropdownSystem;

public interface IExtendedDropdownProvider : IDropdownProvider
{
	string FormatDisplayText(string value, bool selected);

	Sprite GetIcon(string value);

	ImmutableArray<string> GetItemClasses(string value);
}
