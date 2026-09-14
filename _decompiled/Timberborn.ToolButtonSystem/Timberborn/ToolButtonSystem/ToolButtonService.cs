using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.ToolButtonSystem;

public class ToolButtonService : ILoadableSingleton, IPostLoadableSingleton
{
	private readonly ToolbarButtonRetriever _toolbarButtonRetriever;

	private readonly ToolGroupService _toolGroupService;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly List<ToolButton> _toolButtons = new List<ToolButton>();

	private readonly Dictionary<ITool, ToolButton> _toolToButtonMap = new Dictionary<ITool, ToolButton>();

	private readonly List<ToolGroupButton> _toolGroupButtons = new List<ToolGroupButton>();

	private readonly List<IToolbarButton> _rootButtons = new List<IToolbarButton>();

	public ReadOnlyList<ToolButton> ToolButtons => _toolButtons.AsReadOnlyList();

	public ToolButtonService(ToolbarButtonRetriever toolbarButtonRetriever, ToolGroupService toolGroupService, ToolUnlockingService toolUnlockingService)
	{
		_toolbarButtonRetriever = toolbarButtonRetriever;
		_toolGroupService = toolGroupService;
		_toolUnlockingService = toolUnlockingService;
	}

	public void Add(ToolButton toolButton)
	{
		_toolButtons.Add(toolButton);
		_toolToButtonMap[toolButton.Tool] = toolButton;
		UpdateRootTools(toolButton.Tool);
	}

	public void Add(ToolGroupButton toolButton)
	{
		_toolGroupButtons.Add(toolButton);
		_rootButtons.Add(toolButton);
	}

	public void Load()
	{
		_toolGroupService.ToolGroupAssigned += OnToolGroupAssigned;
	}

	public void PostLoad()
	{
		foreach (ToolButton toolButton in _toolButtons)
		{
			toolButton.PostLoad();
			_toolUnlockingService.LockIfNeeded(toolButton.Tool);
		}
		foreach (ToolGroupButton toolGroupButton in _toolGroupButtons)
		{
			toolGroupButton.PostLoad();
		}
	}

	public ToolButton GetToolButton<TTool>(Predicate<TTool> predicate) where TTool : ITool
	{
		return ToolButtons.Single((ToolButton toolButton) => toolButton.Tool is TTool obj && predicate(obj));
	}

	public ToolButton GetToolButton<TTool>() where TTool : ITool
	{
		return GetToolButton((TTool _) => true);
	}

	public ToolGroupButton GetToolGroupButton(ToolButton toolButton)
	{
		return _toolGroupButtons.Single((ToolGroupButton toolGroupButton) => toolGroupButton.HasToolButton(toolButton));
	}

	public bool TryGetNextRootButton(out IToolbarButton nextButton)
	{
		return _toolbarButtonRetriever.TryGetNextVisibleButton(_rootButtons, out nextButton);
	}

	public bool TryGetPreviousRootButton(out IToolbarButton previousButton)
	{
		return _toolbarButtonRetriever.TryGetPreviousVisibleButton(_rootButtons, out previousButton);
	}

	public bool TryGetNextToolButton(out IToolbarButton toolButton)
	{
		if (TryGetActiveToolGroupButton(out var toolGroupButton) && _toolbarButtonRetriever.TryGetNextVisibleButton(toolGroupButton.ToolButtons, out toolButton))
		{
			return true;
		}
		toolButton = null;
		return false;
	}

	public bool TryGetPreviousToolButton(out IToolbarButton toolButton)
	{
		if (TryGetActiveToolGroupButton(out var toolGroupButton) && _toolbarButtonRetriever.TryGetPreviousVisibleButton(toolGroupButton.ToolButtons, out toolButton))
		{
			return true;
		}
		toolButton = null;
		return false;
	}

	private bool TryGetActiveToolGroupButton(out ToolGroupButton toolGroupButton)
	{
		toolGroupButton = _toolGroupButtons.LastOrDefault((ToolGroupButton button) => button.IsActive);
		return toolGroupButton != null;
	}

	private void OnToolGroupAssigned(object sender, ITool tool)
	{
		UpdateRootTools(tool);
	}

	private void UpdateRootTools(ITool tool)
	{
		if (_toolToButtonMap.TryGetValue(tool, out var value))
		{
			bool flag = _rootButtons.Contains(value);
			bool flag2 = _toolGroupService.IsAssignedToAnyGroup(tool);
			if (flag & flag2)
			{
				_rootButtons.Remove(value);
			}
			else if (!flag && !flag2)
			{
				_rootButtons.Add(value);
			}
		}
	}
}
