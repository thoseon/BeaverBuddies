using Bindito.Core;
using Timberborn.Characters;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NeedSystem;

[Context("Game")]
internal class NeedSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NeedManager>().AsTransient();
		Bind<SerializedNeedValueSerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, NeedManager>();
		return builder.Build();
	}
}
