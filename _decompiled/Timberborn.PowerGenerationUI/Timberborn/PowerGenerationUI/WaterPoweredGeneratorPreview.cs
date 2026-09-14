using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using Timberborn.PowerGeneration;

namespace Timberborn.PowerGenerationUI;

internal class WaterPoweredGeneratorPreview : BaseComponent, IAwakableComponent, IPreviewSelectionListener
{
	private readonly WaterPoweredGeneratorPreviewPanel _waterPoweredGeneratorPreviewPanel;

	private WaterPoweredGenerator _waterPoweredGenerator;

	private MechanicalNodeSpec _mechanicalNodeSpec;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	public WaterPoweredGeneratorPreview(WaterPoweredGeneratorPreviewPanel waterPoweredGeneratorPreviewPanel)
	{
		_waterPoweredGeneratorPreviewPanel = waterPoweredGeneratorPreviewPanel;
	}

	public void Awake()
	{
		_waterPoweredGenerator = GetComponent<WaterPoweredGenerator>();
		_mechanicalNodeSpec = GetComponent<MechanicalNodeSpec>();
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
	}

	public void OnPreviewSelect()
	{
		if (_blockObject.Positioned && _blockObject.IsValid())
		{
			int powerOutput = (int)Math.Abs(_waterPoweredGenerator.CalculateGeneratedRotation() * (float)_mechanicalNodeSpec.PowerOutput);
			_waterPoweredGeneratorPreviewPanel.ShowPreview(powerOutput, _blockObjectCenter.WorldCenterGrounded);
		}
		else
		{
			_waterPoweredGeneratorPreviewPanel.HidePreview();
		}
	}

	public void OnPreviewUnselect()
	{
		_waterPoweredGeneratorPreviewPanel.HidePreview();
	}
}
