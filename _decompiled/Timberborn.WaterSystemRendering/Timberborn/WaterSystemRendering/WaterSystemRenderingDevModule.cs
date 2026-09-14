using Timberborn.Debugging;

namespace Timberborn.WaterSystemRendering;

internal class WaterSystemRenderingDevModule : IDevModule
{
	private readonly IWaterMesh _waterMesh;

	private readonly IWaterRenderer _waterRenderer;

	private bool _modelsActive = true;

	private bool _meshesActive = true;

	private bool _texturesActive = true;

	private bool _postprocessingActive = true;

	public WaterSystemRenderingDevModule(IWaterMesh waterMesh, IWaterRenderer waterRenderer)
	{
		_waterMesh = waterMesh;
		_waterRenderer = waterRenderer;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle models: Water", ToggleModels)).AddMethod(DevMethod.Create("Toggle water logic: meshes", ToggleMeshes)).AddMethod(DevMethod.Create("Toggle water logic: textures", ToggleTextures))
			.AddMethod(DevMethod.Create("Toggle water logic: postprocess", TogglePostprocessing))
			.Build();
	}

	private void ToggleMeshes()
	{
		if (_meshesActive)
		{
			_waterRenderer.DisableMeshUpdate();
			_meshesActive = false;
		}
		else
		{
			_waterRenderer.EnableMeshUpdate();
			_meshesActive = true;
		}
	}

	private void ToggleTextures()
	{
		if (_texturesActive)
		{
			_waterRenderer.DisableTextureUpdate();
			_texturesActive = false;
		}
		else
		{
			_waterRenderer.EnableTextureUpdate();
			_texturesActive = true;
		}
	}

	private void TogglePostprocessing()
	{
		if (_postprocessingActive)
		{
			_waterRenderer.DisablePostprocessing();
			_postprocessingActive = false;
		}
		else
		{
			_waterRenderer.EnablePostprocessing();
			_postprocessingActive = true;
		}
	}

	private void ToggleModels()
	{
		if (_modelsActive)
		{
			_waterMesh.Hide();
			_modelsActive = false;
		}
		else
		{
			_waterMesh.Show();
			_modelsActive = true;
		}
	}
}
