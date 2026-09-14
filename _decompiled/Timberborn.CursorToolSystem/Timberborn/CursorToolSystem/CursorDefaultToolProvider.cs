using Timberborn.ToolSystem;

namespace Timberborn.CursorToolSystem;

internal class CursorDefaultToolProvider : IDefaultToolProvider
{
	private readonly CursorTool _cursorTool;

	public ITool DefaultTool => _cursorTool;

	public CursorDefaultToolProvider(CursorTool cursorTool)
	{
		_cursorTool = cursorTool;
	}
}
