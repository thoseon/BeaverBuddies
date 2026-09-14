using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ToolSystem;

public class ToolGroupService : ILoadableSingleton, IInputProcessor
{
	private readonly EventBus _eventBus;

	private readonly ISpecService _specService;

	private readonly InputService _inputService;

	private readonly IDefaultToolProvider _defaultToolProvider;

	private readonly Dictionary<string, ToolGroupSpec> _toolGroups = new Dictionary<string, ToolGroupSpec>();

	private readonly Dictionary<ITool, ToolGroupSpec> _assignedToolGroups = new Dictionary<ITool, ToolGroupSpec>();

	public ToolGroupSpec ActiveToolGroup { get; private set; }

	public event EventHandler<ITool> ToolGroupAssigned;

	public ToolGroupService(EventBus eventBus, ISpecService specService, InputService inputService, IDefaultToolProvider defaultToolProvider)
	{
		_eventBus = eventBus;
		_specService = specService;
		_inputService = inputService;
		_defaultToolProvider = defaultToolProvider;
	}

	public void Load()
	{
		foreach (ToolGroupSpec spec in _specService.GetSpecs<ToolGroupSpec>())
		{
			RegisterGroup(spec);
		}
		_eventBus.Register(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.Cancel && ActiveToolGroup != null)
		{
			ExitToolGroup();
			return true;
		}
		return false;
	}

	public ToolGroupSpec GetGroup(string id)
	{
		if (!_toolGroups.TryGetValue(id, out var value))
		{
			throw new KeyNotFoundException("Unknown ToolGroupSpec: " + id + ".");
		}
		return value;
	}

	public void RegisterGroup(ToolGroupSpec toolGroupSpec)
	{
		_toolGroups.Add(toolGroupSpec.Id, toolGroupSpec);
	}

	public bool IsAssignedToAnyGroup(ITool tool)
	{
		return _assignedToolGroups.ContainsKey(tool);
	}

	public bool IsAssignedToGroup(ITool tool, ToolGroupSpec toolGroup)
	{
		if (_assignedToolGroups.TryGetValue(tool, out var value))
		{
			return toolGroup == value;
		}
		return false;
	}

	public void AssignToGroup(ToolGroupSpec toolGroup, ITool tool)
	{
		Asserts.FieldIsNotNull(this, toolGroup, "toolGroup");
		_assignedToolGroups[tool] = toolGroup;
		ToolGroupAssigned?.Invoke(this, tool);
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.ShouldCloseGroup)
		{
			ExitToolGroupInternal();
		}
		if (toolEnteredEvent.Tool == _defaultToolProvider.DefaultTool)
		{
			PutAsTopInputProcessor();
		}
	}

	public void EnterToolGroup(ToolGroupSpec toolGroup)
	{
		if (toolGroup != ActiveToolGroup)
		{
			ExitToolGroupInternal();
			EnterToolGroupInternal(toolGroup);
		}
	}

	public void ExitToolGroup()
	{
		EnterToolGroup(null);
	}

	private void EnterToolGroupInternal(ToolGroupSpec toolGroup)
	{
		ActiveToolGroup = toolGroup;
		_eventBus.Post(new ToolGroupEnteredEvent(toolGroup));
		if (toolGroup != null)
		{
			_inputService.AddInputProcessor(this);
		}
		else
		{
			_inputService.RemoveInputProcessor(this);
		}
	}

	private void ExitToolGroupInternal()
	{
		ToolGroupSpec activeToolGroup = ActiveToolGroup;
		ActiveToolGroup = null;
		_eventBus.Post(new ToolGroupExitedEvent(activeToolGroup));
	}

	private void PutAsTopInputProcessor()
	{
		if (ActiveToolGroup != null)
		{
			_inputService.RemoveInputProcessor(this);
			_inputService.AddInputProcessor(this);
		}
	}
}
