using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AccessibleNavigation;

[Context("Game")]
[Context("MapEditor")]
internal class AccessibleNavigationConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly AccessibleInitializer _accessibleInitializer;

		public TemplateModuleProvider(AccessibleInitializer accessibleInitializer)
		{
			_accessibleInitializer = accessibleInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDedicatedDecorator(_accessibleInitializer);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AccessibleInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
