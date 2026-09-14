using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.Workshops;

public class NoRecipeStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string NoRecipeLocKey = "Status.Manufactory.NoRecipe";

	private static readonly string NoRecipeShortLocKey = "Status.Manufactory.NoRecipe.Short";

	private readonly ILoc _loc;

	private IRecipeSelector _recipeSelector;

	private StatusToggle _noRecipeStatusToggle;

	private bool _isAutomaticRecipeManufactory;

	public NoRecipeStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_recipeSelector = GetComponent<IRecipeSelector>();
		_noRecipeStatusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("UnspecifiedRecipe", _loc.T(NoRecipeLocKey), _loc.T(NoRecipeShortLocKey));
		_isAutomaticRecipeManufactory = GetComponent<AutomaticRecipeManufactory>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_noRecipeStatusToggle);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateStatusToggle();
		_recipeSelector.RecipeChanged += OnProductionRecipeChanged;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		UpdateStatusToggle();
		_recipeSelector.RecipeChanged -= OnProductionRecipeChanged;
	}

	private void OnProductionRecipeChanged(object sender, EventArgs e)
	{
		UpdateStatusToggle();
	}

	private void UpdateStatusToggle()
	{
		if (base.Enabled && !_recipeSelector.HasCurrentRecipe && !_isAutomaticRecipeManufactory)
		{
			_noRecipeStatusToggle.Activate();
		}
		else
		{
			_noRecipeStatusToggle.Deactivate();
		}
	}
}
