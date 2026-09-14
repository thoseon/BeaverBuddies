using Bindito.Core;
using UnityEngine;

namespace Timberborn.TickSystem;

internal class TickerUnityAdapter : MonoBehaviour
{
	private Ticker _ticker;

	[Inject]
	public void InjectDependencies(Ticker ticker)
	{
		_ticker = ticker;
	}

	public void Update()
	{
		_ticker.Update(Time.deltaTime);
	}
}
