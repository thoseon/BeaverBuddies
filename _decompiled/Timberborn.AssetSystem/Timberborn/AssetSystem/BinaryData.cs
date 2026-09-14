using UnityEngine;

namespace Timberborn.AssetSystem;

public class BinaryData : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private byte[] _bytes;

	public byte[] Bytes => _bytes;

	public void SetData(byte[] bytes)
	{
		_bytes = bytes;
	}
}
