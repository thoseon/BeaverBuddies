namespace Timberborn.WaterSystemRendering;

public interface IWaterRenderer
{
	long UpdateMeshTime { get; }

	long UpdateTexturesTime { get; }

	void EnableMeshUpdate();

	void DisableMeshUpdate();

	void DisableTextureUpdate();

	void EnableTextureUpdate();

	void DisablePostprocessing();

	void EnablePostprocessing();
}
