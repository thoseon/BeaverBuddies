using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.HttpApiSystemUI;

[Context("Game")]
internal class HttpApiSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly HttpApiFragment _httpApiFragment;

		private readonly HttpAdapterFragment _httpAdapterFragment;

		private readonly HttpLeverFragment _httpLeverFragment;

		public EntityPanelModuleProvider(HttpApiFragment httpApiFragment, HttpAdapterFragment httpAdapterFragment, HttpLeverFragment httpLeverFragment)
		{
			_httpApiFragment = httpApiFragment;
			_httpAdapterFragment = httpAdapterFragment;
			_httpLeverFragment = httpLeverFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_httpAdapterFragment, 200);
			builder.AddBottomFragment(_httpLeverFragment, 200);
			builder.AddBottomFragment(_httpApiFragment, 300);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<HttpApiFragment>().AsSingleton();
		Bind<HttpAdapterFragment>().AsSingleton();
		Bind<HttpLeverFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
