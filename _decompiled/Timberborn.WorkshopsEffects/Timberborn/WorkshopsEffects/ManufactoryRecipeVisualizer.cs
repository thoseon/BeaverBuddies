using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.WorkshopsEffects;

internal class ManufactoryRecipeVisualizer : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Manufactory _manufactory;

	private ManufactoryRecipeVisualizerSpec _manufactoryRecipeVisualizerSpec;

	private GameObject _initialModel;

	private readonly Dictionary<string, GameObject> _recipeModels = new Dictionary<string, GameObject>();

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_manufactoryRecipeVisualizerSpec = GetComponent<ManufactoryRecipeVisualizerSpec>();
		_manufactory.RecipeChanged += OnProductionRecipeChanged;
		_initialModel = base.GameObject.FindChild(_manufactoryRecipeVisualizerSpec.InitialModelName);
		DisableComponent();
		InitializeRecipeModels();
		UpdateVisualization();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateVisualization();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		UpdateVisualization();
	}

	private void OnProductionRecipeChanged(object sender, EventArgs e)
	{
		UpdateVisualization();
	}

	private void InitializeRecipeModels()
	{
		foreach (RecipeModel recipeModel in _manufactoryRecipeVisualizerSpec.RecipeModels)
		{
			_recipeModels.Add(recipeModel.RecipeId, base.GameObject.FindChild(recipeModel.ModelName));
		}
	}

	private void UpdateVisualization()
	{
		bool flag = !base.Enabled || !_manufactory.HasCurrentRecipe;
		_initialModel.SetActive(flag);
		string text = (flag ? string.Empty : _manufactory.CurrentRecipe?.Id);
		foreach (var (text3, gameObject2) in _recipeModels)
		{
			gameObject2.SetActive(text3 == text);
		}
	}
}
