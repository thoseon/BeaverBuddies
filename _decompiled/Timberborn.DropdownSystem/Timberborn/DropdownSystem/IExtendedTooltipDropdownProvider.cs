namespace Timberborn.DropdownSystem;

public interface IExtendedTooltipDropdownProvider : IExtendedDropdownProvider, IDropdownProvider
{
	string GetDropdownTooltip(string value);
}
