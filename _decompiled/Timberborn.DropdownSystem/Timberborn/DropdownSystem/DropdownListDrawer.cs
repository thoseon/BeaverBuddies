using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DropdownSystem;

public class DropdownListDrawer : ILoadableSingleton, IInputProcessor
{
	private static readonly int MaxHeight = 510;

	private readonly InputService _inputService;

	private readonly ScrollBarInitializationService _scrollBarInitializationService;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private ScrollView _items;

	private VisualElement _parent;

	private int _isMouseOver;

	private bool _ignoreWorldInput;

	public bool DropdownVisible => _root.IsDisplayed();

	public DropdownListDrawer(InputService inputService, ScrollBarInitializationService scrollBarInitializationService, RootVisualElementProvider rootVisualElementProvider, EventBus eventBus)
	{
		_inputService = inputService;
		_scrollBarInitializationService = scrollBarInitializationService;
		_rootVisualElementProvider = rootVisualElementProvider;
		_eventBus = eventBus;
	}

	public void Load()
	{
		VisualElement e = _rootVisualElementProvider.Create("DropdownListDrawer", "Core/DropdownItems", 2);
		_root = e.Q<VisualElement>("DropdownItemsWrapper");
		_items = _root.Q<ScrollView>("DropdownItems");
		_scrollBarInitializationService.InitializeVisualElement(_items);
		_root.ToggleDisplayStyle(visible: false);
	}

	public bool ProcessInput()
	{
		if ((!_ignoreWorldInput || _inputService.MouseOverUI) && (_inputService.Cancel || ((_inputService.MainMouseButtonDown || _inputService.ScrollWheelActive) && _isMouseOver == 0)))
		{
			HideDropdown();
			return true;
		}
		return false;
	}

	public void ShowDropdown(VisualElement parent, IEnumerable<VisualElement> items)
	{
		HideDropdown();
		_parent = parent;
		_inputService.AddInputProcessor(this);
		_parent.RegisterCallback<MouseEnterEvent>(delegate
		{
			MouseEntered();
		});
		_parent.RegisterCallback<MouseLeaveEvent>(delegate
		{
			MouseLeft();
		});
		_items.RegisterCallback<MouseEnterEvent>(delegate
		{
			MouseEntered();
		});
		_items.RegisterCallback<MouseLeaveEvent>(delegate
		{
			MouseLeft();
		});
		_isMouseOver = 1;
		_root.ToggleDisplayStyle(visible: true);
		foreach (VisualElement item in items)
		{
			_items.Add(item);
		}
		CalculateDimensions();
	}

	public void HideDropdown()
	{
		if (DropdownVisible)
		{
			_inputService.RemoveInputProcessor(this);
			_items.Clear();
			_root.ToggleDisplayStyle(visible: false);
			_parent.UnregisterCallback<MouseEnterEvent>(delegate
			{
				MouseEntered();
			});
			_parent.UnregisterCallback<MouseLeaveEvent>(delegate
			{
				MouseLeft();
			});
			_items.UnregisterCallback<MouseEnterEvent>(delegate
			{
				MouseEntered();
			});
			_items.UnregisterCallback<MouseLeaveEvent>(delegate
			{
				MouseLeft();
			});
			_isMouseOver = 0;
			_parent = null;
			_eventBus.Post(new DropdownHiddenEvent());
		}
	}

	public void IgnoreWorldInput(bool ignoreWorldInput)
	{
		_ignoreWorldInput = ignoreWorldInput;
	}

	private void MouseEntered()
	{
		_isMouseOver++;
	}

	private void MouseLeft()
	{
		_isMouseOver--;
	}

	private void CalculateDimensions()
	{
		Vector2 vector = _parent.LocalToWorld(_parent.resolvedStyle.translate);
		_root.style.left = vector.x;
		_root.style.width = _parent.resolvedStyle.width;
		float num = vector.y + _parent.resolvedStyle.height;
		float height = _root.parent.resolvedStyle.height;
		bool num2 = height - num > 150f;
		float val = (num2 ? (height - num - 20f) : (vector.y + 20f));
		_root.style.maxHeight = Math.Min(MaxHeight, val);
		if (num2)
		{
			_root.style.top = num;
			_root.style.bottom = new StyleLength(StyleKeyword.Auto);
		}
		else
		{
			_root.style.top = new StyleLength(StyleKeyword.Auto);
			_root.style.bottom = height - vector.y;
		}
	}
}
