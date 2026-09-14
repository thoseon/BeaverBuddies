using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.WellbeingUI;

namespace Timberborn.BonusSystemUI;

[Context("Game")]
internal class BonusSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly BonusManagerDebugFragment _bonusManagerDebugFragment;

		public EntityPanelModuleProvider(BonusManagerDebugFragment bonusManagerDebugFragment)
		{
			_bonusManagerDebugFragment = bonusManagerDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_bonusManagerDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BonusManagerDebugFragment>().AsSingleton();
		Bind<NeedPenaltyEffectDescriber>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<INeedEffectDescriber>().To<NeedPenaltyEffectDescriber>().AsSingleton();
	}
}
