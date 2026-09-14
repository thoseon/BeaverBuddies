using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.ToolButtonSystem;

namespace Timberborn.BeaversUI;

internal class BeaverGeneratorButton : IBottomBarElementsProvider
{
	private static readonly string ToolImageKey = "BeaverGeneratorTool";

	private readonly BeaverGeneratorTool _beaverGeneratorTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	public BeaverGeneratorButton(BeaverGeneratorTool beaverGeneratorTool, ToolButtonFactory toolButtonFactory)
	{
		_beaverGeneratorTool = beaverGeneratorTool;
		_toolButtonFactory = toolButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolButton toolButton = _toolButtonFactory.CreateGrouplessRed(_beaverGeneratorTool, ToolImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
	}
}
