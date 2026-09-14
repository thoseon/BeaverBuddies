using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.PrefabOptimization;

internal record VegetationMaterialProperties : IMaterialProperties
{
	public Color32 Color { get; private init; }

	private static readonly string UseEmissionKeyword = "_USE_EMISSION";

	private static readonly int ColorId = Shader.PropertyToID("_Color");

	private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

	private static readonly int MetallicGlossMapId = Shader.PropertyToID("_MetallicGlossMap");

	private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");

	private static readonly int DetailMapId = Shader.PropertyToID("_DetailMap");

	private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

	private static readonly int WindModifierId = Shader.PropertyToID("_WindModifier");

	private static readonly int SwayStrengthId = Shader.PropertyToID("_SwayStrength");

	private static readonly int SwaySpeedId = Shader.PropertyToID("_SwaySpeed");

	private static readonly int SwayExponentId = Shader.PropertyToID("_SwayExponent");

	private static readonly int FlutterStrengthId = Shader.PropertyToID("_FlutterStrength");

	private static readonly int FlutterSpeedId = Shader.PropertyToID("_FlutterSpeed");

	private static readonly int FlutterExponentId = Shader.PropertyToID("_FlutterExponent");

	private static readonly int FlutterThresholdId = Shader.PropertyToID("_FlutterThreshold");

	private static readonly int DetailUseColorId = Shader.PropertyToID("_DetailUseColor");

	private static readonly int DetailColorBoostId = Shader.PropertyToID("_DetailColorBoost");

	private static readonly int EnableDetailId = Shader.PropertyToID("_EnableDetail");

	private Texture2D _mainTex;

	private Texture2D _bumpMap;

	private Texture2D _metallicGlossMap;

	private Texture2D _detailMap;

	private Color32 _emissionColor;

	private bool _useEmission;

	private float _windModifier;

	private float _swayStrength;

	private float _swaySpeed;

	private float _swayExponent;

	private float _flutterStrength;

	private float _flutterSpeed;

	private float _flutterExponent;

	private float _flutterThreshold;

	private float _detailUseColor;

	private float _detailColorBoost;

	private float _enableDetail;

	private bool _enableInstancing;

	public static VegetationMaterialProperties FromMaterial(Material material)
	{
		return new VegetationMaterialProperties
		{
			Color = material.GetColor(ColorId),
			_mainTex = (Texture2D)material.GetTexture(MainTexId),
			_metallicGlossMap = (Texture2D)material.GetTexture(MetallicGlossMapId),
			_bumpMap = (Texture2D)material.GetTexture(BumpMapId),
			_detailMap = (Texture2D)material.GetTexture(DetailMapId),
			_emissionColor = material.GetColor(EmissionColorId),
			_useEmission = material.IsKeywordEnabled(UseEmissionKeyword),
			_windModifier = material.GetFloat(WindModifierId),
			_swayStrength = material.GetFloat(SwayStrengthId),
			_swaySpeed = material.GetFloat(SwaySpeedId),
			_swayExponent = material.GetFloat(SwayExponentId),
			_flutterStrength = material.GetFloat(FlutterStrengthId),
			_flutterSpeed = material.GetFloat(FlutterSpeedId),
			_flutterExponent = material.GetFloat(FlutterExponentId),
			_flutterThreshold = material.GetFloat(FlutterThresholdId),
			_detailUseColor = material.GetFloat(DetailUseColorId),
			_detailColorBoost = material.GetFloat(DetailColorBoostId),
			_enableDetail = material.GetFloat(EnableDetailId),
			_enableInstancing = material.enableInstancing
		};
	}

	public void ApplyToMaterial(Material material)
	{
		material.SetColor(ColorId, Color);
		material.SetTexture(MainTexId, _mainTex);
		material.SetTexture(BumpMapId, _bumpMap);
		material.SetTexture(MetallicGlossMapId, _metallicGlossMap);
		material.SetTexture(DetailMapId, _detailMap);
		material.SetColor(EmissionColorId, _emissionColor);
		material.SetKeyword(new LocalKeyword(material.shader, UseEmissionKeyword), _useEmission);
		material.SetFloat(WindModifierId, _windModifier);
		material.SetFloat(SwayStrengthId, _swayStrength);
		material.SetFloat(SwaySpeedId, _swaySpeed);
		material.SetFloat(SwayExponentId, _swayExponent);
		material.SetFloat(FlutterStrengthId, _flutterStrength);
		material.SetFloat(FlutterSpeedId, _flutterSpeed);
		material.SetFloat(FlutterExponentId, _flutterExponent);
		material.SetFloat(FlutterThresholdId, _flutterThreshold);
		material.SetFloat(DetailUseColorId, _detailUseColor);
		material.SetFloat(DetailColorBoostId, _detailColorBoost);
		material.SetFloat(EnableDetailId, _enableDetail);
		material.enableInstancing = _enableInstancing;
	}

	public IMaterialProperties GetWithoutColor()
	{
		return this with
		{
			Color = UnityEngine.Color.white
		};
	}
}
