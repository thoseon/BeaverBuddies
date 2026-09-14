using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal class GoodVisualization : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private readonly GoodIconVisualizer _goodIconVisualizer;

	private readonly MaterialHeightCutoffSetter _materialHeightCutoffSetter;

	private BuildingModel _buildingModel;

	private EntityMaterials _entityMaterials;

	private MeshFilter _meshFilter;

	private MeshRenderer _meshRenderer;

	private GameObject _visualization;

	private Mesh _emptyMesh;

	public GoodVisualization(GoodIconVisualizer goodIconVisualizer, MaterialHeightCutoffSetter materialHeightCutoffSetter)
	{
		_goodIconVisualizer = goodIconVisualizer;
		_materialHeightCutoffSetter = materialHeightCutoffSetter;
	}

	public void Awake()
	{
		_buildingModel = GetComponent<BuildingModel>();
		_entityMaterials = GetComponent<EntityMaterials>();
		CreateVisualizationObject();
	}

	public void SetLocalPosition(Vector3 position)
	{
		_visualization.transform.localPosition = position;
	}

	public void SetPositionAndRotation(Vector3 position, Quaternion quaternion)
	{
		_visualization.transform.SetPositionAndRotation(position, quaternion);
	}

	public void SetMaterial(Material material, float heightCutoff)
	{
		SetNewMaterial(material);
		SetHeightCutoff(heightCutoff);
	}

	public void SetMesh(Mesh mesh)
	{
		_meshFilter.sharedMesh = mesh;
	}

	public void SetIcon(GoodSpec goodSpec)
	{
		SetIcon(goodSpec, goodSpec.ContainerColor);
	}

	public void SetIcon(GoodSpec goodSpec, Color color)
	{
		if ((bool)_meshRenderer.material)
		{
			_goodIconVisualizer.ShowIcon(_meshRenderer.material, goodSpec, color);
		}
	}

	public void Clear()
	{
		ClearMaterial();
		_meshFilter.sharedMesh = _emptyMesh;
	}

	public void DeleteEntity()
	{
		Object.Destroy(_emptyMesh);
	}

	private void CreateVisualizationObject()
	{
		_visualization = new GameObject("GoodVisualization");
		_visualization.transform.SetParent(_buildingModel.FinishedModel.transform, worldPositionStays: false);
		_emptyMesh = new Mesh();
		_meshFilter = _visualization.AddComponent<MeshFilter>();
		_meshRenderer = _visualization.AddComponent<MeshRenderer>();
		Clear();
	}

	private void SetHeightCutoff(float cutoff)
	{
		float y = GetComponent<BlockObjectCenter>().WorldCenterGrounded.y;
		_materialHeightCutoffSetter.SetCutoff(_meshRenderer.material, y + cutoff);
	}

	private void SetNewMaterial(Material material)
	{
		ClearMaterial();
		_meshRenderer.material = material;
		_entityMaterials.AddMaterial(_visualization.transform, _meshRenderer.material);
	}

	private void ClearMaterial()
	{
		if ((bool)_meshRenderer.material)
		{
			_entityMaterials.DestroyMaterial(_meshRenderer.material);
		}
	}
}
