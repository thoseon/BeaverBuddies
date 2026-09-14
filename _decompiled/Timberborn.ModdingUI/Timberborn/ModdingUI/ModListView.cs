using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Modding;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ModdingUI;

public class ModListView
{
	private readonly IModItemFactory _modItemFactory;

	private readonly ModSorter _modSorter;

	private readonly ModWarningUpdater _modWarningUpdater = new ModWarningUpdater();

	private ScrollView _scrollView;

	private readonly Dictionary<Mod, ModItem> _modItems = new Dictionary<Mod, ModItem>();

	public event EventHandler ListChanged;

	public ModListView(IModItemFactory modItemFactory, ModSorter modSorter)
	{
		_modItemFactory = modItemFactory;
		_modSorter = modSorter;
	}

	public void Initialize(VisualElement root, IEnumerable<Mod> mods)
	{
		_scrollView = root.Q<ScrollView>();
		root.Q<Button>("ResetOrderButton").RegisterCallback<ClickEvent>(delegate
		{
			ResetPriorities();
		});
		BuildList(mods);
		SortList();
	}

	public void ResetScroll()
	{
		_scrollView.scrollOffset = Vector2.zero;
	}

	public void Update()
	{
		foreach (ModItem value in _modItems.Values)
		{
			value.Update();
		}
	}

	private void ResetPriorities()
	{
		foreach (Mod key in _modItems.Keys)
		{
			ModPlayerPrefsHelper.ResetModPriority(key);
		}
		ListChanged?.Invoke(this, EventArgs.Empty);
		SortList();
	}

	private void SortList()
	{
		foreach (Mod item in _modSorter.Sort(_modItems.Keys))
		{
			_modItems[item].Root.BringToFront();
		}
		_modWarningUpdater.Update(_modItems);
	}

	private void BuildList(IEnumerable<Mod> mods)
	{
		foreach (Mod mod in mods)
		{
			ModItem modItem = _modItemFactory.CreateModItem(mod, OnPriorityIncreased, OnPriorityDecreased);
			modItem.ModToggled += OnModToggled;
			_modItems.Add(mod, modItem);
			_scrollView.Add(modItem.Root);
		}
	}

	private void OnPriorityIncreased(Mod mod, bool moveToTop)
	{
		ModItem modItem = _modItems[mod];
		int num = modItem.Root.parent.IndexOf(modItem.Root);
		if (num > 0)
		{
			if (moveToTop)
			{
				MoveToTop(mod, num, modItem.Root.parent);
			}
			else
			{
				IncreasePriority(mod, num, modItem.Root.parent);
			}
		}
	}

	private void MoveToTop(Mod mod, int index, VisualElement parent)
	{
		ModPlayerPrefsHelper.SetModPriority(mod, ModPlayerPrefsHelper.GetModPriority(mod) + index);
		while (index > 0)
		{
			ModPlayerPrefsHelper.DecreaseModPriority(GetModFromElement(parent.ElementAt(index - 1)));
			index--;
		}
		ListChanged?.Invoke(this, EventArgs.Empty);
		SortList();
	}

	private void IncreasePriority(Mod mod, int index, VisualElement parent)
	{
		Mod modFromElement = GetModFromElement(parent.ElementAt(index - 1));
		ModPlayerPrefsHelper.IncreaseModPriority(mod);
		ModPlayerPrefsHelper.DecreaseModPriority(modFromElement);
		ListChanged?.Invoke(this, EventArgs.Empty);
		SortList();
	}

	private void OnPriorityDecreased(Mod mod, bool moveToBottom)
	{
		ModItem modItem = _modItems[mod];
		int num = modItem.Root.parent.IndexOf(modItem.Root);
		if (num < modItem.Root.parent.childCount - 1)
		{
			if (moveToBottom)
			{
				MoveToBottom(mod, num, modItem.Root.parent);
			}
			else
			{
				DecreasePriority(mod, num, modItem.Root.parent);
			}
		}
	}

	private void MoveToBottom(Mod mod, int index, VisualElement parent)
	{
		ModPlayerPrefsHelper.SetModPriority(mod, ModPlayerPrefsHelper.GetModPriority(mod) - (parent.childCount - index - 1));
		while (index < parent.childCount - 1)
		{
			ModPlayerPrefsHelper.IncreaseModPriority(GetModFromElement(parent.ElementAt(index + 1)));
			index++;
		}
		ListChanged?.Invoke(this, EventArgs.Empty);
		SortList();
	}

	private void DecreasePriority(Mod mod, int index, VisualElement parent)
	{
		Mod modFromElement = GetModFromElement(parent.ElementAt(index + 1));
		ModPlayerPrefsHelper.DecreaseModPriority(mod);
		ModPlayerPrefsHelper.IncreaseModPriority(modFromElement);
		ListChanged?.Invoke(this, EventArgs.Empty);
		SortList();
	}

	private void OnModToggled(object sender, EventArgs e)
	{
		ListChanged?.Invoke(this, EventArgs.Empty);
		_modWarningUpdater.Update(_modItems);
	}

	private Mod GetModFromElement(VisualElement element)
	{
		return _modItems.Single((KeyValuePair<Mod, ModItem> x) => x.Value.Root == element).Key;
	}
}
