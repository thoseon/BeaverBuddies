using System;
using System.Collections.Generic;
using System.Reflection;
using Timberborn.Common;

namespace Timberborn.SingletonSystem;

public class EventBus : IPostLoadableSingleton
{
	private readonly SubscriptionRegistry _subscriptions = new SubscriptionRegistry();

	private readonly Dictionary<Type, List<MethodInfo>> _methodCache = new Dictionary<Type, List<MethodInfo>>();

	private readonly Queue<Action> _pendingActions = new Queue<Action>();

	private readonly Queue<object> _earlyEvents = new Queue<object>();

	private bool _ready;

	private bool _posting;

	public void PostLoad()
	{
		while (_earlyEvents.Count > 0)
		{
			PostNow(_earlyEvents.Dequeue());
		}
		_ready = true;
	}

	public void Register(object subscriber)
	{
		InvokeOrEnqueue(delegate
		{
			RegisterNow(subscriber);
		});
	}

	public void Unregister(object subscriber)
	{
		InvokeOrEnqueue(delegate
		{
			UnregisterNow(subscriber);
		});
	}

	public void Unregister(ReadOnlyList<object> subscribers)
	{
		foreach (object subscriber in subscribers)
		{
			if (_subscriptions.IsSubscriber(subscriber))
			{
				InvokeOrEnqueue(delegate
				{
					UnregisterNow(subscriber);
				});
			}
		}
	}

	public void Post(object eventObject)
	{
		if (_ready)
		{
			PostNow(eventObject);
		}
		else
		{
			_earlyEvents.Enqueue(eventObject);
		}
	}

	private void PostNow(object eventObject)
	{
		bool posting = _posting;
		_posting = true;
		try
		{
			Type type = eventObject.GetType();
			IEnumerable<Subscription> enumerable = _subscriptions.Get(type);
			if (enumerable == null)
			{
				return;
			}
			foreach (Subscription item in enumerable)
			{
				item.Action(eventObject);
			}
		}
		finally
		{
			if (!posting)
			{
				InvokePendingActions();
			}
			_posting = posting;
		}
	}

	private void InvokeOrEnqueue(Action action)
	{
		if (_posting)
		{
			_pendingActions.Enqueue(action);
		}
		else
		{
			action();
		}
	}

	private void InvokePendingActions()
	{
		while (_pendingActions.Count > 0)
		{
			_pendingActions.Dequeue()();
		}
	}

	private void RegisterNow(object subscriber)
	{
		Type type = subscriber.GetType();
		if (_methodCache.TryGetValue(type, out var value))
		{
			foreach (MethodInfo item in value)
			{
				RegisterMethod(subscriber, item);
			}
			return;
		}
		List<MethodInfo> list = new List<MethodInfo>();
		_methodCache[type] = list;
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.GetCustomAttribute<OnEventAttribute>() != null)
			{
				if (!methodInfo.IsPublic)
				{
					throw new ArgumentException(type.FullName + ".$" + methodInfo.Name + " must be public");
				}
				list.Add(methodInfo);
				RegisterMethod(subscriber, methodInfo);
			}
		}
	}

	private void RegisterMethod(object subscriber, MethodInfo method)
	{
		if (method.ReturnType != typeof(void))
		{
			throw new ArgumentException($"Can't register {method} of {subscriber.GetType()}. " + "Listening methods must return void.");
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != 1)
		{
			throw new ArgumentException($"Can't register {method} of {subscriber.GetType()}. " + "Listening methods must have exactly one parameter.");
		}
		Type parameterType = parameters[0].ParameterType;
		Action<object> action = delegate(object e)
		{
			method.Invoke(subscriber, new object[1] { e });
		};
		_subscriptions.Add(parameterType, subscriber, action);
	}

	private void UnregisterNow(object subscriber)
	{
		_subscriptions.RemoveAll(subscriber);
	}
}
