using System;
using System.Collections.Generic;
using System.Linq;

namespace Timberborn.SingletonSystem;

internal class SubscriptionRegistry
{
	private readonly Dictionary<Type, Dictionary<object, Action<object>>> _subscriptions = new Dictionary<Type, Dictionary<object, Action<object>>>();

	private readonly HashSet<object> _subscribers = new HashSet<object>();

	public void Add(Type eventType, object subscriber, Action<object> action)
	{
		if (!_subscriptions.ContainsKey(eventType))
		{
			_subscriptions[eventType] = new Dictionary<object, Action<object>>();
		}
		if (_subscriptions[eventType].ContainsKey(subscriber))
		{
			throw new ArgumentException($"There is already an entry for {subscriber}");
		}
		_subscriptions[eventType].Add(subscriber, action);
		_subscribers.Add(subscriber);
	}

	public void RemoveAll(object subscriber)
	{
		_subscribers.Remove(subscriber);
		foreach (KeyValuePair<Type, Dictionary<object, Action<object>>> subscription in _subscriptions)
		{
			subscription.Value.Remove(subscriber);
		}
	}

	public IEnumerable<Subscription> Get(Type eventType)
	{
		if (!_subscriptions.ContainsKey(eventType))
		{
			return null;
		}
		return GetSubscriptions(eventType);
	}

	public bool IsSubscriber(object subscriber)
	{
		return _subscribers.Contains(subscriber);
	}

	private IEnumerable<Subscription> GetSubscriptions(Type eventType)
	{
		return _subscriptions[eventType].Select((KeyValuePair<object, Action<object>> subscription) => new Subscription(subscription.Key, subscription.Value));
	}
}
