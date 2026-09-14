using Bindito.Core;
using Timberborn.BuilderHubSystem;
using Timberborn.SimpleOutputBuildingsUI;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuilderHubSystemUI;

[Context("Game")]
internal class BuilderHubSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuilderHubSpec, SimpleOutputInventoryFragmentEnabler>();
		return builder.Build();
	}
}
