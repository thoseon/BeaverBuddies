using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MortalSystem;

[Context("Game")]
internal class MortalSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DeadRootBehavior>().AsTransient();
		Bind<Mortal>().AsTransient();
		Bind<DeadStatus>().AsTransient();
		Bind<MortalNeeder>().AsTransient();
		Bind<Temporal>().AsTransient();
		Bind<LongLastingCorpsesService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<IDevModule>().To<CharacterKiller>().AsSingleton();
		MultiBind<IDevModule>().To<LongLastingCorpsesDevModule>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MortalSpec, Mortal>();
		builder.AddDecorator<Mortal, MortalNeeder>();
		builder.AddDecorator<DeadStatusSpec, DeadStatus>();
		builder.AddDecorator<TemporalSpec, Temporal>();
		return builder.Build();
	}
}
