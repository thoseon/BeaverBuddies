using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TimbermeshAnimations;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ShaftModelFactory : ILoadableSingleton
{
	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly ShaftFrameFactory _shaftFrameFactory;

	private readonly TemplateService _templateService;

	private ModularShaftPartsSpec _modularShaftPartsSpec;

	public ShaftModelFactory(OptimizedPrefabInstantiator optimizedPrefabInstantiator, ShaftFrameFactory shaftFrameFactory, TemplateService templateService)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
		_shaftFrameFactory = shaftFrameFactory;
		_templateService = templateService;
	}

	public void Load()
	{
		_modularShaftPartsSpec = _templateService.GetSingle<ModularShaftPartsSpec>();
	}

	public void BuildNonStackable(ShaftVariant variant, GameObject root)
	{
		Build(variant, root, isStackable: false);
	}

	public void BuildStackable(ShaftVariant variant, GameObject root)
	{
		Build(variant, root, isStackable: true);
	}

	private void Build(ShaftVariant variant, GameObject root, bool isStackable)
	{
		ShaftAssembly assembly = default(ShaftAssembly);
		bool isReversed = false;
		variant = GetVariantWithSymmetricalHorizontalEnds(variant);
		if (TryGetMainDirection(variant, out var mainDirection))
		{
			Direction3D direction = mainDirection.RotateHorizontally(Orientation.Cw90);
			Direction3D direction2 = mainDirection.RotateHorizontally(Orientation.Cw270);
			byte rotation = variant.GetRotation(direction);
			byte rotation2 = variant.GetRotation(direction2);
			byte rotation3 = variant.GetRotation(mainDirection);
			isReversed = rotation3 == 2;
			if (rotation > 0)
			{
				assembly.ConnectLeft(rotation != rotation3);
			}
			if (rotation2 > 0)
			{
				assembly.ConnectRight(rotation2 != rotation3);
			}
			byte rotation4 = variant.GetRotation(Direction3D.Top);
			if (rotation4 > 0)
			{
				assembly.ConnectTop(rotation4 != rotation3);
			}
			byte rotation5 = variant.GetRotation(Direction3D.Bottom);
			if (rotation5 > 0)
			{
				assembly.ConnectBottom(rotation5 != rotation3);
			}
			byte rotation6 = variant.GetRotation(mainDirection.Across());
			if (rotation6 > 0)
			{
				assembly.ConnectOpposite(rotation6 != rotation3);
			}
		}
		else
		{
			byte rotation7 = variant.GetRotation(Direction3D.Top);
			byte rotation8 = variant.GetRotation(Direction3D.Bottom);
			if (rotation7 > 0 && rotation8 > 0)
			{
				assembly.ConnectTopAndBottomOnly(rotation7 != rotation8);
				isReversed = ((rotation7 == rotation8) ? (rotation8 == 1) : (rotation7 == 2));
			}
			else if (rotation8 > 0)
			{
				assembly.ConnectBottomOnly();
				isReversed = rotation8 == 1;
			}
			else if (rotation7 > 0)
			{
				assembly.ConnectTopOnly();
				isReversed = rotation7 == 1;
			}
		}
		assembly.Optimize();
		InstantiateMovingParts(assembly, mainDirection, root, isReversed);
		InstantiateFrame(assembly, variant, mainDirection, root, isStackable);
	}

	private static bool TryGetMainDirection(ShaftVariant variant, out Direction3D mainDirection)
	{
		if (variant.Down > 0)
		{
			mainDirection = Direction3D.Down;
			return true;
		}
		if (variant.Left > 0)
		{
			mainDirection = Direction3D.Left;
			return true;
		}
		if (variant.Up > 0)
		{
			mainDirection = Direction3D.Up;
			return true;
		}
		if (variant.Right > 0)
		{
			mainDirection = Direction3D.Right;
			return true;
		}
		mainDirection = Direction3D.Down;
		return false;
	}

	private static ShaftVariant GetVariantWithSymmetricalHorizontalEnds(ShaftVariant variant)
	{
		if (variant.Top == 0 && variant.Bottom == 0)
		{
			if (variant.Left > 0 && variant.Right == 0 && variant.Up == 0 && variant.Down == 0)
			{
				return variant.AddSymmetryRight();
			}
			if (variant.Right > 0 && variant.Left == 0 && variant.Up == 0 && variant.Down == 0)
			{
				return variant.AddSymmetryLeft();
			}
			if (variant.Up > 0 && variant.Down == 0 && variant.Left == 0 && variant.Right == 0)
			{
				return variant.AddSymmetryDown();
			}
			if (variant.Down > 0 && variant.Up == 0 && variant.Left == 0 && variant.Right == 0)
			{
				return variant.AddSymmetryUp();
			}
		}
		return variant;
	}

	private void InstantiateMovingParts(ShaftAssembly assembly, Direction3D mainDirection, GameObject root, bool isReversed)
	{
		Direction3D direction = mainDirection.RotateHorizontally(Orientation.Cw90);
		Direction3D direction2 = mainDirection.RotateHorizontally(Orientation.Cw270);
		Direction3D direction3 = mainDirection.Across();
		if (assembly.ShowMainGearSmall && !assembly.ShowMainGearLarge)
		{
			Instantiate(_modularShaftPartsSpec.GearSmall, root, mainDirection, isReversed);
		}
		else if (assembly.ShowMainGearLarge)
		{
			Instantiate(_modularShaftPartsSpec.GearLarge, root, mainDirection, isReversed);
		}
		if (assembly.ShowGearInner)
		{
			Instantiate(_modularShaftPartsSpec.GearInner, root, mainDirection, isReversed);
		}
		else if (assembly.ShowGearInnerLong)
		{
			Instantiate(_modularShaftPartsSpec.GearInnerLong, root, mainDirection, isReversed);
		}
		else if (assembly.ShowAxleInnerLong)
		{
			Instantiate(_modularShaftPartsSpec.AxleInnerLong, root, direction3, !isReversed);
		}
		else if (assembly.ShowGearInnerThrough)
		{
			Instantiate(_modularShaftPartsSpec.GearInnerThrough, root, mainDirection, isReversed);
		}
		if (assembly.ShowGearInnerOpposite)
		{
			Instantiate(_modularShaftPartsSpec.GearInnerOpposite, root, mainDirection, isReversed);
		}
		if (assembly.ShowBottomGearBase && !assembly.ShowGearBottomLarge)
		{
			Instantiate(_modularShaftPartsSpec.GearBottomBase, root, mainDirection, !isReversed);
		}
		if (assembly.ShowOppositeGearSmall)
		{
			Instantiate(_modularShaftPartsSpec.GearSmall, root, direction3, isReversed);
		}
		if (assembly.ShowLeftGearSmall)
		{
			Instantiate(_modularShaftPartsSpec.GearSmall, root, direction, isReversed);
		}
		if (assembly.ShowLeftGearMedium)
		{
			Instantiate(_modularShaftPartsSpec.GearMedium, root, direction, !isReversed);
		}
		if (assembly.ShowRightGearSmall)
		{
			Instantiate(_modularShaftPartsSpec.GearSmall, root, direction2, isReversed);
		}
		if (assembly.ShowRightGearMedium)
		{
			Instantiate(_modularShaftPartsSpec.GearMedium, root, direction2, !isReversed);
		}
		if (assembly.ShowGearBottomSmall)
		{
			Instantiate(_modularShaftPartsSpec.GearBottomSmall, root, Direction3D.Up, isReversed);
		}
		if (assembly.ShowGearBottomLarge)
		{
			Instantiate(_modularShaftPartsSpec.GearBottomLarge, root, Direction3D.Up, !isReversed);
		}
		if (assembly.ShowGearTopSmall)
		{
			Instantiate(_modularShaftPartsSpec.GearTopSmall, root, Direction3D.Up, !isReversed);
		}
		if (assembly.ShowGearTopLarge)
		{
			Instantiate(_modularShaftPartsSpec.GearTopLarge, root, Direction3D.Up, !isReversed);
		}
		if (assembly.ShowAxleVertical)
		{
			Instantiate(_modularShaftPartsSpec.AxleVertical, root, Direction3D.Up, !isReversed);
		}
		if (assembly.ShowAxleHorizontal)
		{
			Instantiate(_modularShaftPartsSpec.AxleHorizontal, root, mainDirection, isReversed);
		}
	}

	private void InstantiateFrame(ShaftAssembly assembly, ShaftVariant variant, Direction3D mainDirection, GameObject root, bool isStackable)
	{
		bool showMainGearSmall = assembly.ShowMainGearSmall;
		bool down = variant.Down > 0 || (showMainGearSmall && mainDirection == Direction3D.Down);
		bool left = variant.Left > 0 || (showMainGearSmall && mainDirection == Direction3D.Left);
		bool up = variant.Up > 0 || (showMainGearSmall && mainDirection == Direction3D.Up);
		bool right = variant.Right > 0 || (showMainGearSmall && mainDirection == Direction3D.Right);
		bool bottom = assembly.ShowBottomGearBase || assembly.ShowGearBottomLarge || assembly.ShowGearBottomSmall || assembly.ShowAxleVertical;
		FrameVariant variant2 = new FrameVariant(down, left, up, right, bottom, isStackable);
		GameObject frame = _shaftFrameFactory.GetFrame(variant2);
		frame.transform.SetParent(root.transform);
		frame.SetActive(value: true);
	}

	private void Instantiate(AssetRef<GameObject> gameObject, GameObject root, Direction3D direction, bool reverseRotation)
	{
		GameObject gameObject2 = _optimizedPrefabInstantiator.Instantiate(gameObject.Asset, root.transform);
		Quaternion localRotation = Quaternion.AngleAxis(direction.ToHorizontalAngle(), Vector3.up);
		gameObject2.transform.SetLocalPositionAndRotation(Vector3.zero, localRotation);
		gameObject2.GetComponent<IAnimator>().PlayBackwards = reverseRotation;
		gameObject2.SetActive(value: true);
	}
}
