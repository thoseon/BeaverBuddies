using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusAlertAddedEvent
{
	public string StatusAlert { get; }

	public Sprite StatusSprite { get; }

	public StatusAlertAddedEvent(string statusAlert, Sprite statusSprite)
	{
		StatusAlert = statusAlert;
		StatusSprite = statusSprite;
	}
}
