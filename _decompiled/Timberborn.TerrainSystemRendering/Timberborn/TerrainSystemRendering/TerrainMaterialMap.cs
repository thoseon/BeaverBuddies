using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

[MapEditorTickable]
public class TerrainMaterialMap : ITickableSingleton, ILoadableSingleton, IPostLoadableSingleton, ILateUpdatableSingleton
{
	private readonly struct MapDataChange
	{
		public Vector3Int Coordinates { get; }

		public float OldValue { get; }

		public float NewValue { get; }

		public MapDataChange(Vector3Int coordinates, float oldValue, float newValue)
		{
			Coordinates = coordinates;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	private static readonly int ProjectionUVScaleProperty = Shader.PropertyToID("_ProjectionUVScale");

	private static readonly int TerrainMaterialMapScaleProperty = Shader.PropertyToID("_TerrainMaterialMapScale");

	private static readonly int DesertIntensityMapProperty = Shader.PropertyToID("_DesertIntensityMap");

	private static readonly int CutoutAndFieldMapProperty = Shader.PropertyToID("_CutoutAndFieldMap");

	private static readonly int ContaminationMapProperty = Shader.PropertyToID("_ContaminationMap");

	private static readonly int DesertTextureProperty = Shader.PropertyToID("_DesertTex");

	private static readonly int WetFieldTextureProperty = Shader.PropertyToID("_WetFieldTex");

	private static readonly int DryFieldTextureProperty = Shader.PropertyToID("_DryFieldTex");

	private static readonly int BlendingNoiseProperty = Shader.PropertyToID("_BlendingNoise");

	private static readonly int BlendingNoiseScaleProperty = Shader.PropertyToID("_BlendingNoiseScale");

	private static readonly int BlendingNoiseMultiplierProperty = Shader.PropertyToID("_BlendingNoiseMultiplier");

	private static readonly int BlendingSoftnessProperty = Shader.PropertyToID("_BlendingSoftness");

	private static readonly int BlendingMarginProperty = Shader.PropertyToID("_BlendingMargin");

	private static readonly int AltitudeCeilingProperty = Shader.PropertyToID("_AltitudeCeiling");

	private static readonly int AltitudeMultiplierProperty = Shader.PropertyToID("_AltitudeMultiplierTex");

	private static readonly int DesertAltitudeMultiplierProperty = Shader.PropertyToID("_DesertAltitudeMultiplierTex");

	private static readonly int CutoutMarginProperty = Shader.PropertyToID("_CutoutMargin");

	private static readonly float ChangeThreshold = 0.003921569f;

	private readonly ITerrainService _terrainService;

	private readonly ISpecService _specService;

	private Vector4 _textureScaleAsVector;

	private Texture2DArray _desertIntensityMapTexture;

	private Texture2DArray _cutoutAndFieldMapTexture;

	private Texture2DArray _contaminationMapTexture;

	private Texture2D _bufferTexture;

	private Vector3Int _textureSize;

	private PixelData[][] _desertIntensityMap;

	private PixelData[][] _cutoutAndFieldMap;

	private PixelData[][] _contaminations;

	private readonly Queue<MapDataChange> _desertMapChanges = new Queue<MapDataChange>();

	private readonly Queue<MapDataChange> _contaminationMapChanges = new Queue<MapDataChange>();

	private readonly HashSet<byte> _desertIntensityLayersToUpdate = new HashSet<byte>();

	private readonly HashSet<byte> _cutoutAndFieldLayersToUpdate = new HashSet<byte>();

	private readonly HashSet<byte> _contaminationLayersToUpdate = new HashSet<byte>();

	private bool _fieldOrCutoutTextureInvalid;

	private bool _applyTextureChanges;

	private bool _applyContaminationTextureChanges;

	private TerrainMaterialMapSpec _terrainMaterialMapSpec;

	public TerrainMaterialMap(ITerrainService terrainService, ISpecService specService)
	{
		_terrainService = terrainService;
		_specService = specService;
	}

	public void LateUpdateSingleton()
	{
		if (_fieldOrCutoutTextureInvalid)
		{
			_fieldOrCutoutTextureInvalid = false;
			UpdateFieldAndCutoutTexture();
		}
		if (_desertIntensityLayersToUpdate.Count > 0)
		{
			ApplyDesertIntensityMapChanges();
			_desertIntensityLayersToUpdate.Clear();
		}
		if (_cutoutAndFieldLayersToUpdate.Count > 0)
		{
			ApplyCutoutAndFieldMapChanges();
			_cutoutAndFieldLayersToUpdate.Clear();
		}
		if (_contaminationLayersToUpdate.Count > 0)
		{
			ApplyContaminationTextureChanges();
			_contaminationLayersToUpdate.Clear();
		}
		if (Application.isEditor)
		{
			UpdateShaderProperties();
		}
	}

	public void Tick()
	{
		ProcessDesertTextureChanges();
		ProcessContaminationTextureChanges();
	}

	public void Load()
	{
		_terrainMaterialMapSpec = _specService.GetSingleSpec<TerrainMaterialMapSpec>();
		InitializeTextures();
		_terrainService.FieldOrCutoutChanged += OnFieldOrCutoutChanged;
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	public void PostLoad()
	{
		UpdateFieldAndCutoutTexture();
		ProcessDesertTextureChanges();
		ProcessDesertTextureChanges();
		ProcessContaminationTextureChanges();
		ProcessContaminationTextureChanges();
		UpdateShaderProperties();
	}

	public void ResetDesertMap()
	{
		for (int i = 0; i < _textureSize.z; i++)
		{
			int num = _desertIntensityMap[i].Length;
			for (int j = 0; j < num; j++)
			{
				_desertIntensityMap[i][j] = new PixelData(1f, 1f);
			}
			_desertIntensityLayersToUpdate.Add((byte)i);
		}
		_desertMapChanges.Clear();
	}

	public void ResetContaminationMap()
	{
		for (int i = 0; i < _textureSize.z; i++)
		{
			int num = _contaminations[i].Length;
			for (int j = 0; j < num; j++)
			{
				_contaminations[i][j] = new PixelData(0f, 0f);
			}
			_contaminationLayersToUpdate.Add((byte)i);
		}
		_contaminationMapChanges.Clear();
	}

	public void SetDesertIntensity(Vector3Int coordinates, float desertIntensity)
	{
		float desertIntensity2 = GetDesertIntensity(coordinates);
		if (Math.Abs(desertIntensity2 - desertIntensity) > ChangeThreshold)
		{
			_desertMapChanges.Enqueue(new MapDataChange(coordinates, desertIntensity2, desertIntensity));
		}
	}

	public void SetSoilContamination(Vector3Int coordinates, float contamination)
	{
		float gNormalized = _contaminations[coordinates.z][coordinates.y * _textureSize.x + coordinates.x].GNormalized;
		if (Math.Abs(gNormalized - contamination) > ChangeThreshold)
		{
			_contaminationMapChanges.Enqueue(new MapDataChange(coordinates, gNormalized, contamination));
		}
	}

	public float GetDesertIntensity(Vector3Int coordinates)
	{
		return _desertIntensityMap[coordinates.z][coordinates.y * _textureSize.x + coordinates.x].GNormalized;
	}

	private void InitializeTextures()
	{
		_textureSize = _terrainService.Size;
		_textureScaleAsVector = new Vector4(1f / (float)_textureSize.x, 1f / (float)_textureSize.y);
		_desertIntensityMapTexture = new Texture2DArray(_textureSize.x, _textureSize.y, _textureSize.z, TextureFormat.RG16, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		_cutoutAndFieldMapTexture = new Texture2DArray(_textureSize.x, _textureSize.y, _textureSize.z, TextureFormat.RG16, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		_contaminationMapTexture = new Texture2DArray(_textureSize.x, _textureSize.y, _textureSize.z, TextureFormat.RG16, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		_bufferTexture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.RG16, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		_desertIntensityMap = new PixelData[_textureSize.z][];
		_cutoutAndFieldMap = new PixelData[_textureSize.z][];
		_contaminations = new PixelData[_textureSize.z][];
		for (int i = 0; i < _textureSize.z; i++)
		{
			_desertIntensityMap[i] = new PixelData[_textureSize.x * _textureSize.y];
			for (int j = 0; j < _textureSize.x * _textureSize.y; j++)
			{
				_desertIntensityMap[i][j] = new PixelData(1f, 1f);
			}
			_contaminations[i] = new PixelData[_textureSize.x * _textureSize.y];
			_cutoutAndFieldMap[i] = new PixelData[_textureSize.x * _textureSize.y];
			_contaminationLayersToUpdate.Add((byte)i);
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		if (!change.SetTerrain)
		{
			for (int num = change.To + 1; num > change.From; num--)
			{
				Vector3Int coordinates = change.Coordinates.ToVector3Int(num);
				_desertMapChanges.Enqueue(new MapDataChange(coordinates, 1f, 1f));
				_contaminationMapChanges.Enqueue(new MapDataChange(coordinates, 0f, 0f));
			}
		}
	}

	private void ProcessDesertTextureChanges()
	{
		int count = _desertMapChanges.Count;
		int num = 0;
		while (num++ < count)
		{
			UpdateDesertTexture(_desertMapChanges.Dequeue());
		}
	}

	private void ProcessContaminationTextureChanges()
	{
		int count = _contaminationMapChanges.Count;
		int num = 0;
		while (num++ < count)
		{
			UpdateContaminationTexture(_contaminationMapChanges.Dequeue());
		}
	}

	private void UpdateDesertTexture(in MapDataChange mapDataChange)
	{
		float oldValue = mapDataChange.OldValue;
		float newValue = mapDataChange.NewValue;
		Vector3Int coordinates = mapDataChange.Coordinates;
		_desertIntensityMap[coordinates.z][coordinates.y * _textureSize.x + coordinates.x] = new PixelData(oldValue, newValue);
		_desertIntensityLayersToUpdate.Add((byte)coordinates.z);
		if (newValue != oldValue)
		{
			_desertMapChanges.Enqueue(new MapDataChange(coordinates, newValue, newValue));
		}
	}

	private void UpdateContaminationTexture(in MapDataChange contaminationMapChange)
	{
		float oldValue = contaminationMapChange.OldValue;
		float newValue = contaminationMapChange.NewValue;
		Vector3Int coordinates = contaminationMapChange.Coordinates;
		_contaminations[coordinates.z][coordinates.y * _textureSize.x + coordinates.x] = new PixelData(oldValue, newValue);
		_contaminationLayersToUpdate.Add((byte)coordinates.z);
		if (newValue != oldValue)
		{
			_contaminationMapChanges.Enqueue(new MapDataChange(coordinates, newValue, newValue));
		}
	}

	private void OnFieldOrCutoutChanged(object sender, Vector3Int coordinates)
	{
		float r = (_terrainService.CellIsField(coordinates) ? 1f : 0f);
		float g = (_terrainService.CellIsCutout(coordinates) ? 0f : 1f);
		_cutoutAndFieldMap[coordinates.z][coordinates.y * _textureSize.x + coordinates.x] = new PixelData(r, g);
		_cutoutAndFieldLayersToUpdate.Add((byte)coordinates.z);
	}

	private void UpdateFieldAndCutoutTexture()
	{
		for (int i = 0; i < _textureSize.z; i++)
		{
			for (int j = 0; j < _textureSize.y; j++)
			{
				for (int k = 0; k < _textureSize.x; k++)
				{
					Vector3Int cellCoordinates = new Vector3Int(k, j, i);
					float r = (_terrainService.CellIsField(cellCoordinates) ? 1f : 0f);
					float g = (_terrainService.CellIsCutout(cellCoordinates) ? 0f : 1f);
					_cutoutAndFieldMap[i][j * _textureSize.x + k] = new PixelData(r, g);
				}
			}
			_cutoutAndFieldLayersToUpdate.Add((byte)i);
		}
	}

	private void UpdateShaderProperties()
	{
		Shader.SetGlobalFloat(ProjectionUVScaleProperty, 1f / (float)WorldTiling.HorizontalTileSize);
		Shader.SetGlobalVector(TerrainMaterialMapScaleProperty, _textureScaleAsVector);
		Shader.SetGlobalTexture(DesertIntensityMapProperty, _desertIntensityMapTexture);
		Shader.SetGlobalTexture(CutoutAndFieldMapProperty, _cutoutAndFieldMapTexture);
		Shader.SetGlobalTexture(ContaminationMapProperty, _contaminationMapTexture);
		Shader.SetGlobalTexture(DesertTextureProperty, _terrainMaterialMapSpec.DesertTexture.Asset);
		Shader.SetGlobalTexture(WetFieldTextureProperty, _terrainMaterialMapSpec.WetFieldTexture.Asset);
		Shader.SetGlobalTexture(DryFieldTextureProperty, _terrainMaterialMapSpec.DryFieldTexture.Asset);
		Shader.SetGlobalTexture(BlendingNoiseProperty, _terrainMaterialMapSpec.BlendingNoise.Asset);
		Shader.SetGlobalFloat(BlendingNoiseScaleProperty, _terrainMaterialMapSpec.BlendingNoiseScale);
		Shader.SetGlobalFloat(BlendingNoiseMultiplierProperty, _terrainMaterialMapSpec.BlendingNoiseMultiplier);
		Shader.SetGlobalFloat(BlendingSoftnessProperty, _terrainMaterialMapSpec.BlendingSoftness);
		Shader.SetGlobalFloat(BlendingMarginProperty, _terrainMaterialMapSpec.BlendingMargin);
		Shader.SetGlobalFloat(AltitudeCeilingProperty, _terrainMaterialMapSpec.AltitudeCeiling);
		Shader.SetGlobalTexture(AltitudeMultiplierProperty, _terrainMaterialMapSpec.AltitudeMultiplier.Asset);
		Shader.SetGlobalTexture(DesertAltitudeMultiplierProperty, _terrainMaterialMapSpec.DesertAltitudeMultiplier.Asset);
		Shader.SetGlobalFloat(CutoutMarginProperty, _terrainMaterialMapSpec.CutoutMargin);
	}

	private void ApplyDesertIntensityMapChanges()
	{
		foreach (byte item in _desertIntensityLayersToUpdate)
		{
			_bufferTexture.SetPixelData(_desertIntensityMap[item], 0);
			_bufferTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			Graphics.CopyTexture(_bufferTexture, 0, 0, _desertIntensityMapTexture, item, 0);
		}
	}

	private void ApplyCutoutAndFieldMapChanges()
	{
		foreach (byte item in _cutoutAndFieldLayersToUpdate)
		{
			_bufferTexture.SetPixelData(_cutoutAndFieldMap[item], 0);
			_bufferTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			Graphics.CopyTexture(_bufferTexture, 0, 0, _cutoutAndFieldMapTexture, item, 0);
		}
	}

	private void ApplyContaminationTextureChanges()
	{
		foreach (byte item in _contaminationLayersToUpdate)
		{
			_bufferTexture.SetPixelData(_contaminations[item], 0);
			_bufferTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			Graphics.CopyTexture(_bufferTexture, 0, 0, _contaminationMapTexture, item, 0);
		}
	}
}
