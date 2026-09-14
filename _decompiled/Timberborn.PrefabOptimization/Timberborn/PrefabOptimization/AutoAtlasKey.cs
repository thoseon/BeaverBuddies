using System;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public struct AutoAtlasKey(Texture originalMainTex, Texture originalBumpMap, Texture originalColorMask, Texture originalAmbientOcclusion, Texture originalMetallicGlossMap, Texture originalLightingMap) : IEquatable<AutoAtlasKey>
{
	private readonly Texture _originalMainTex = originalMainTex;

	private readonly Texture _originalBumpMap = originalBumpMap;

	private readonly Texture _originalColorMask = originalColorMask;

	private readonly Texture _originalAmbientOcclusion = originalAmbientOcclusion;

	private readonly Texture _originalMetallicGlossMap = originalMetallicGlossMap;

	private readonly Texture _originalLightingMap = originalLightingMap;

	public bool Equals(AutoAtlasKey other)
	{
		if (object.Equals(_originalMainTex, other._originalMainTex) && object.Equals(_originalBumpMap, other._originalBumpMap) && object.Equals(_originalColorMask, other._originalColorMask) && object.Equals(_originalAmbientOcclusion, other._originalAmbientOcclusion) && object.Equals(_originalMetallicGlossMap, other._originalMetallicGlossMap))
		{
			return object.Equals(_originalLightingMap, other._originalLightingMap);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is AutoAtlasKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((((((_originalMainTex != null) ? _originalMainTex.GetHashCode() : 0) * 397) ^ ((_originalBumpMap != null) ? _originalBumpMap.GetHashCode() : 0)) * 397) ^ ((_originalColorMask != null) ? _originalColorMask.GetHashCode() : 0)) * 397) ^ ((_originalAmbientOcclusion != null) ? _originalAmbientOcclusion.GetHashCode() : 0)) * 397) ^ ((_originalMetallicGlossMap != null) ? _originalMetallicGlossMap.GetHashCode() : 0)) * 397) ^ ((_originalLightingMap != null) ? _originalLightingMap.GetHashCode() : 0);
	}

	public override string ToString()
	{
		return string.Format("{0}: {1}", "_originalMainTex", _originalMainTex) + string.Format(", {0}: {1}", "_originalBumpMap", _originalBumpMap) + string.Format(", {0}: {1}", "_originalColorMask", _originalColorMask) + string.Format(", {0}: {1}", "_originalAmbientOcclusion", _originalAmbientOcclusion) + string.Format(", {0}: {1}", "_originalMetallicGlossMap", _originalMetallicGlossMap) + string.Format(", {0}: {1}", "_originalLightingMap", _originalLightingMap);
	}

	public static bool operator ==(AutoAtlasKey left, AutoAtlasKey right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(AutoAtlasKey left, AutoAtlasKey right)
	{
		return !left.Equals(right);
	}
}
