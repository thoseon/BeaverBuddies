using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.DecalSystem;

internal class UserDecalService
{
	private readonly UserDecalTextureRepository _userDecalTextureRepository;

	public UserDecalService(UserDecalTextureRepository userDecalTextureRepository)
	{
		_userDecalTextureRepository = userDecalTextureRepository;
	}

	public IEnumerable<DecalSpec> GetCustomDecals(string category)
	{
		IEnumerable<Texture2D> enumerable = _userDecalTextureRepository.LoadCustomTextures(category);
		foreach (Texture2D item in enumerable)
		{
			yield return new DecalSpec
			{
				FactionId = string.Empty,
				Category = category,
				Texture = new AssetRef<Texture2D>(string.Empty, new Lazy<Texture2D>(item))
			};
		}
	}
}
