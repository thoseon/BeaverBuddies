using System;
using JetBrains.Annotations;

namespace Timberborn.SingletonSystem;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class OnEventAttribute : Attribute
{
}
