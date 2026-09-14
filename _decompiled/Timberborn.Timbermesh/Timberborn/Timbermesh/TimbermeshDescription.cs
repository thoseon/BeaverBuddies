using UnityEngine;

namespace Timberborn.Timbermesh;

public class TimbermeshDescription : MonoBehaviour
{
	[SerializeField]
	private string _modelName;

	public string ModelName => _modelName;

	public void SetModelName(string modelName)
	{
		_modelName = modelName;
	}
}
