namespace Timberborn.ToolButtonSystem;

public interface IToolbarButton
{
	bool IsVisible { get; }

	bool IsActive { get; }

	void Select();
}
