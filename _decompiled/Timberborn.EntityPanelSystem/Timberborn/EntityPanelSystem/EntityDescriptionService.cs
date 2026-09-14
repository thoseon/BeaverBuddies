using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

public class EntityDescriptionService
{
	private static readonly string DescriptionBackgroundClass = "bg-sub-box--blue";

	private static readonly string SingleSectionClass = "description-text--single-section";

	private static readonly string MiddleSectionRootClass = "content-row-centered";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RowShader _rowShader;

	private readonly List<EntityDescription> _entityDescriptions = new List<EntityDescription>();

	private readonly List<IEntityDescriber> _entityDescribersCache = new List<IEntityDescriber>();

	public EntityDescriptionService(VisualElementLoader visualElementLoader, RowShader rowShader)
	{
		_visualElementLoader = visualElementLoader;
		_rowShader = rowShader;
	}

	public void DescribeAsSingleSection(BaseComponent subject, VisualElement root)
	{
		Describe(subject, root, singleSection: true, "");
	}

	public void DescribeAsSeparateSections(BaseComponent subject, VisualElement root, string startingDescription = "")
	{
		Describe(subject, root, singleSection: false, startingDescription);
	}

	private void Describe(BaseComponent subject, VisualElement root, bool singleSection, string startingDescription)
	{
		subject.GetComponents(_entityDescribersCache);
		IOrderedEnumerable<EntityDescription> collection = from description in _entityDescribersCache.SelectMany((IEntityDescriber describer) => describer.DescribeEntity())
			orderby description.Order
			select description;
		_entityDescriptions.AddRange(collection);
		AddSections(subject, root, singleSection, startingDescription);
		_entityDescribersCache.Clear();
		_entityDescriptions.Clear();
	}

	private void AddSections(BaseComponent subject, VisualElement root, bool singleSection, string startingDescription)
	{
		DescribeHeader(subject, root, singleSection);
		DescribeProduction(root);
		DescribeText(root, singleSection, startingDescription);
		DescribeBottomSections(root, singleSection);
	}

	private void DescribeHeader(BaseComponent subject, VisualElement root, bool singleSection)
	{
		if (!singleSection)
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescriptionHeader");
			LabeledEntity component = subject.GetComponent<LabeledEntity>();
			visualElement.Q<Label>("Title").text = component.DisplayName;
			visualElement.Q<Image>("Icon").sprite = component.Image;
			AddMiddleSections(visualElement.Q<VisualElement>("AdditionalInfo"));
			root.Add(visualElement);
			return;
		}
		string elementName = "Game/EntityDescription/DescriptionEmptySection";
		VisualElement visualElement2 = _visualElementLoader.LoadVisualElement(elementName);
		visualElement2.AddToClassList(MiddleSectionRootClass);
		AddMiddleSections(visualElement2);
		if (visualElement2.childCount > 0)
		{
			root.Add(visualElement2);
		}
	}

	private void AddMiddleSections(VisualElement middleSectionRoot)
	{
		foreach (EntityDescription item in _entityDescriptions.Where((EntityDescription description) => description.MiddleSection))
		{
			middleSectionRoot.Add(item.Section);
		}
	}

	private void DescribeProduction(VisualElement root)
	{
		if (_entityDescriptions.Any((EntityDescription description) => description.ProductionSection))
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/ProductionItems");
			AddElements(_entityDescriptions.Where((EntityDescription description) => description.Input && description.Output), visualElement, "InputAndOutput");
			_rowShader.ShadeRows(visualElement.Q<VisualElement>("InputAndOutput"));
			bool flag = AddElements(_entityDescriptions.Where((EntityDescription description) => description.Input && !description.Output), visualElement, "Inputs");
			bool flag2 = AddElements(_entityDescriptions.Where((EntityDescription description) => !description.Input && description.Output), visualElement, "Outputs");
			visualElement.Q<Image>("InputOrOutputIcon").ToggleDisplayStyle(flag | flag2);
			SetTime(visualElement);
			root.Add(visualElement);
		}
	}

	private static bool AddElements(IEnumerable<EntityDescription> descriptions, VisualElement root, string name)
	{
		VisualElement visualElement = root.Q<VisualElement>(name);
		bool result = false;
		foreach (EntityDescription description in descriptions)
		{
			visualElement.Add(description.Section);
			result = true;
		}
		return result;
	}

	private void SetTime(VisualElement productionItems)
	{
		EntityDescription entityDescription = _entityDescriptions.FirstOrDefault((EntityDescription description) => description.Time != null);
		Label label = productionItems.Q<Label>("Time");
		label.ToggleDisplayStyle(entityDescription != null);
		if (entityDescription != null)
		{
			label.text = entityDescription.Time;
		}
	}

	private void DescribeBottomSections(VisualElement root, bool singleSection)
	{
		if (_entityDescriptions.Any((EntityDescription description) => description.BottomSection))
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescriptionBottomSection");
			AddElements(_entityDescriptions.Where((EntityDescription description) => description.BottomSection), visualElement, "BottomSection");
			visualElement.EnableInClassList(DescriptionBackgroundClass, !singleSection);
			root.Add(visualElement);
		}
	}

	private void DescribeText(VisualElement root, bool singleSection, string startingDescription)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/DescriptionText");
		visualElement.EnableInClassList(SingleSectionClass, singleSection);
		visualElement.EnableInClassList(DescriptionBackgroundClass, !singleSection);
		IEnumerable<EntityDescription> entityDescriptions = _entityDescriptions.Where((EntityDescription description) => description.TextSection);
		Label label = visualElement.Q<Label>("Description");
		bool flag = Describe(entityDescriptions, label, startingDescription);
		label.ToggleDisplayStyle(flag);
		IEnumerable<EntityDescription> entityDescriptions2 = _entityDescriptions.Where((EntityDescription description) => description.FlavorSection);
		Label label2 = visualElement.Q<Label>("Flavor");
		bool flag2 = Describe(entityDescriptions2, label2, "");
		label2.ToggleDisplayStyle(flag2);
		if (flag2 | flag)
		{
			root.Add(visualElement);
		}
	}

	private bool Describe(IEnumerable<EntityDescription> entityDescriptions, Label textLabel, string startingDescription)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(startingDescription))
		{
			stringBuilder.AppendLine(startingDescription);
		}
		foreach (EntityDescription entityDescription in entityDescriptions)
		{
			stringBuilder.AppendLine(entityDescription.Content);
		}
		string value = (textLabel.text = stringBuilder.ToStringWithoutNewLineEnd());
		return !string.IsNullOrEmpty(value);
	}
}
