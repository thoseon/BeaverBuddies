using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.ToolButtonSystem;

namespace Timberborn.BotsUI;

public class BotGeneratorButton : IBottomBarElementsProvider
{
	private static readonly string ToolImageKey = "BotGeneratorTool";

	private readonly BotGeneratorTool _botGeneratorTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	public BotGeneratorButton(BotGeneratorTool botGeneratorTool, ToolButtonFactory toolButtonFactory)
	{
		_botGeneratorTool = botGeneratorTool;
		_toolButtonFactory = toolButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolButton toolButton = _toolButtonFactory.CreateGrouplessRed(_botGeneratorTool, ToolImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
	}
}
