using UnityEngine;

namespace Timberborn.RootProviders;

public class RootObjectProvider
{
	public GameObject CreateRootObject(string name)
	{
		return new GameObject(name);
	}
}
