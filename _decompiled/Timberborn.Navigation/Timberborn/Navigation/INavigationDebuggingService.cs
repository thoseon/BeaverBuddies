using UnityEngine;

namespace Timberborn.Navigation;

public interface INavigationDebuggingService
{
	string InfoAt(Vector3 position);
}
