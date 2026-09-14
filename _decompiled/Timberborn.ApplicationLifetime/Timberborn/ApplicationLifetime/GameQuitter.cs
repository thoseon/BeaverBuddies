using UnityEngine;

namespace Timberborn.ApplicationLifetime;

public static class GameQuitter
{
	public static void Quit()
	{
		Application.Quit();
	}
}
