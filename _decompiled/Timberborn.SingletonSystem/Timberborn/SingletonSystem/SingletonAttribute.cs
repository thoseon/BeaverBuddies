using System;
using JetBrains.Annotations;

namespace Timberborn.SingletonSystem;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
[MeansImplicitUse]
public class SingletonAttribute : Attribute
{
}
