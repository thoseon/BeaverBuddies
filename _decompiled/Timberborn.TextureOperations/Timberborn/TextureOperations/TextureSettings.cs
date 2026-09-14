using UnityEngine;

namespace Timberborn.TextureOperations;

public class TextureSettings
{
	public class Builder
	{
		private bool _linear;

		private bool _generateMipmap = true;

		private int _mipmapCount = -1;

		private bool _ignoreMipmapLimits;

		private FilterMode _filterMode = FilterMode.Trilinear;

		private TextureWrapMode _wrapMode;

		private TextureFormat _textureFormat = TextureFormat.RGBA32;

		private int _anisoLevel = 16;

		private int _width = 1;

		private int _height = 1;

		private string _name = "Texture";

		public Builder SetSpritePreset()
		{
			return SetGenerateMipmap(generateMipmap: false).SetWrapMode(TextureWrapMode.Clamp);
		}

		public Builder SetNormalMapPreset()
		{
			return SetLinear(linear: true);
		}

		public Builder SetLinear(bool linear)
		{
			_linear = linear;
			return this;
		}

		public Builder SetGenerateMipmap(bool generateMipmap)
		{
			_generateMipmap = generateMipmap;
			return this;
		}

		public Builder SetMipmapCount(int mipmapCount)
		{
			_mipmapCount = mipmapCount;
			return this;
		}

		public Builder SetIgnoreMipmapLimits(bool ignoreMipmapLimits)
		{
			_ignoreMipmapLimits = ignoreMipmapLimits;
			return this;
		}

		public Builder SetFilterMode(FilterMode filterMode)
		{
			_filterMode = filterMode;
			return this;
		}

		public Builder SetWrapMode(TextureWrapMode wrapMode)
		{
			_wrapMode = wrapMode;
			return this;
		}

		public Builder SetTextureFormat(TextureFormat textureFormat)
		{
			_textureFormat = textureFormat;
			return this;
		}

		public Builder SetAnisoLevel(int anisoLevel)
		{
			_anisoLevel = anisoLevel;
			return this;
		}

		public Builder SetSize(int width, int height)
		{
			_width = width;
			_height = height;
			return this;
		}

		public Builder SetName(string name)
		{
			_name = name;
			return this;
		}

		public TextureSettings Build()
		{
			return new TextureSettings(_linear, _generateMipmap, _mipmapCount, _ignoreMipmapLimits, _filterMode, _wrapMode, _textureFormat, _anisoLevel, _width, _height, _name);
		}
	}

	public bool Linear { get; }

	public bool GenerateMipmap { get; }

	public int MipmapCount { get; }

	public bool IgnoreMipmapLimits { get; }

	public FilterMode FilterMode { get; }

	public TextureWrapMode WrapMode { get; }

	public TextureFormat TextureFormat { get; }

	public int AnisoLevel { get; }

	public int Width { get; }

	public int Height { get; }

	public string Name { get; }

	private TextureSettings(bool linear, bool generateMipmap, int mipmapCount, bool ignoreMipmapLimits, FilterMode filterMode, TextureWrapMode wrapMode, TextureFormat textureFormat, int anisoLevel, int width, int height, string name)
	{
		Linear = linear;
		GenerateMipmap = generateMipmap;
		MipmapCount = mipmapCount;
		IgnoreMipmapLimits = ignoreMipmapLimits;
		FilterMode = filterMode;
		WrapMode = wrapMode;
		TextureFormat = textureFormat;
		AnisoLevel = anisoLevel;
		Width = width;
		Height = height;
		Name = name;
	}
}
