using Bindito.Core;
using Timberborn.CharacterControlSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterControlSystemUI;

[Context("Game")]
internal class CharacterControlSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly CharacterControlFragment _characterControlFragment;

		public EntityPanelModuleProvider(CharacterControlFragment characterControlFragment)
		{
			_characterControlFragment = characterControlFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_characterControlFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ControllableCharacterDropdownProvider>().AsTransient();
		Bind<CharacterControlFragment>().AsSingleton();
		Bind<CharacterControlDestinationPicker>().AsSingleton();
		Bind<ControllableCharacterAnimations>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ControllableCharacter, ControllableCharacterDropdownProvider>();
		return builder.Build();
	}
}
