using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Timberborn.Common;

public static class Asserts
{
	[AssertionMethod]
	public static void FieldIsNull<T>(T owner, object value, string name) where T : class
	{
		if (value != null)
		{
			string objectName = GetObjectName(owner);
			throw new InvalidOperationException("Field of " + objectName + " named " + name + " isn't null");
		}
	}

	[AssertionMethod]
	public static void FieldIsNotNull<T>(T owner, object value, string name) where T : class
	{
		if (value == null)
		{
			string objectName = GetObjectName(owner);
			throw new InvalidOperationException("Field of " + objectName + " named " + name + " is null");
		}
	}

	[AssertionMethod]
	public static void ValueIsInRange<T>(T value, T inclusiveMin, T inclusiveMax, string name) where T : IComparable<T>
	{
		if (value.CompareTo(inclusiveMin) < 0 || value.CompareTo(inclusiveMax) > 0)
		{
			throw new ArgumentException($"Value {value} named {name} is outside of the range {inclusiveMin} to {inclusiveMax}");
		}
	}

	[AssertionMethod]
	public static void CollectionContains<T>(IReadOnlyCollection<T> collection, T item, string collectionName)
	{
		if (!collection.Contains(item))
		{
			throw new ArgumentException($"Collection {collectionName} does not contain item {item}");
		}
	}

	[AssertionMethod]
	public static void CollectionIsEmpty<T>(IReadOnlyCollection<T> collection, string collectionName)
	{
		if (collection == null)
		{
			throw new ArgumentException("Collection " + collectionName + " is null!");
		}
		if (collection.Count != 0)
		{
			throw new ArgumentException("Collection " + collectionName + " is not empty!");
		}
	}

	[AssertionMethod]
	public static void CollectionIsNotEmpty<T>(IReadOnlyCollection<T> collection, string collectionName)
	{
		if (collection == null)
		{
			throw new ArgumentException("Collection " + collectionName + " is null!");
		}
		if (collection.Count == 0)
		{
			throw new ArgumentException("Collection " + collectionName + " is empty!");
		}
	}

	[AssertionMethod]
	public static void IsFalse<T>(T owner, bool value, string name) where T : class
	{
		if (value)
		{
			string objectName = GetObjectName(owner);
			throw new InvalidOperationException("Field of " + objectName + " named " + name + " is true");
		}
	}

	[AssertionMethod]
	public static void IsTrue<T>(T owner, bool value, string name) where T : class
	{
		if (!value)
		{
			string objectName = GetObjectName(owner);
			throw new InvalidOperationException("Field of " + objectName + " named " + name + " is false");
		}
	}

	private static string GetObjectName<T>(T obj) where T : class
	{
		if (!(obj is MonoBehaviour monoBehaviour))
		{
			return typeof(T).Name;
		}
		return monoBehaviour.name + " (" + monoBehaviour.GetType().Name + ")";
	}
}
