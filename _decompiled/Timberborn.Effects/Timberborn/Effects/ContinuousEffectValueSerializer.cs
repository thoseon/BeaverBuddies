using System.Collections.Generic;
using System.Linq;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using Timberborn.Persistence;
using UnityEngine;

namespace Timberborn.Effects;

public class ContinuousEffectValueSerializer : IValueSerializer<ContinuousEffect>
{
	private static readonly PropertyKey<string> NeedIdKey = new PropertyKey<string>("NeedId");

	private static readonly PropertyKey<float> PointsPerHourKey = new PropertyKey<float>("PointsPerHour");

	private readonly FactionNeedService _factionNeedService;

	private IEnumerable<string> NeedSpecs => _factionNeedService.Needs.Select((NeedSpec need) => need.Id);

	public ContinuousEffectValueSerializer(FactionNeedService factionNeedService)
	{
		_factionNeedService = factionNeedService;
	}

	public void Serialize(ContinuousEffect value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(NeedIdKey, value.NeedId);
		objectSaver.Set(PointsPerHourKey, value.PointsPerHour);
	}

	public Obsoletable<ContinuousEffect> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		string text = objectLoader.Get(NeedIdKey);
		float pointsPerHour = objectLoader.Get(PointsPerHourKey);
		if (!NeedSpecs.Contains(text))
		{
			Debug.Log("Need " + text + " found in save doesn't exist, ignoring it.");
			return default(Obsoletable<ContinuousEffect>);
		}
		return new ContinuousEffect(text, pointsPerHour);
	}
}
