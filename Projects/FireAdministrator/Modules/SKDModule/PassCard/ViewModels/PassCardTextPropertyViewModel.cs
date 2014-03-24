﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Designer.ElementProperties.ViewModels;
using FiresecClient.SKDHelpers;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardTextPropertyViewModel : TextBlockPropertiesViewModel
	{
		public PassCardTextPropertyViewModel(ElementPassCardTextProperty element)
			: base(element)
		{
			Title = "Свойства фигуры: Текстовое свойство";
			PropertyTypes = new ObservableCollection<PassCardTextPropertyType>(Enum.GetValues(typeof(PassCardTextPropertyType)).Cast<PassCardTextPropertyType>());
		}

		public ObservableCollection<PassCardTextPropertyType> PropertyTypes { get; private set; }
		private PassCardTextPropertyType _selectedPropertyType;
		public PassCardTextPropertyType SelectedPropertyType
		{
			get { return _selectedPropertyType; }
			set
			{
				_selectedPropertyType = value;
				OnPropertyChanged(() => SelectedPropertyType);
				OnPropertyChanged(() => IsAdditionalColumn);
				if (SelectedPropertyType != PassCardTextPropertyType.Additional)
					SelectedAdditionalColumnType = null;
			}
		}

		public ObservableCollection<AdditionalColumnType> AdditionalColumnTypes { get; private set; }
		private AdditionalColumnType _selectedAdditionalColumnType;
		public AdditionalColumnType SelectedAdditionalColumnType
		{
			get { return _selectedAdditionalColumnType; }
			set
			{
				_selectedAdditionalColumnType = value;
				OnPropertyChanged(() => SelectedAdditionalColumnType);
			}
		}

		public bool IsAdditionalColumn
		{
			get { return SelectedPropertyType == PassCardTextPropertyType.Additional; }
		}

		protected override void CopyProperties()
		{
			var filter = new AdditionalColumnTypeFilter()
			{
				WithDeleted = DeletedType.Not,
				Type = AdditionalColumnDataType.Text,
			};
			AdditionalColumnTypes = new ObservableCollection<AdditionalColumnType>(AdditionalColumnTypeHelper.Get(filter));
			base.CopyProperties();
			SelectedPropertyType = ((ElementPassCardTextProperty)ElementTextBlock).PropertyType;
			SelectedAdditionalColumnType = SelectedPropertyType == PassCardTextPropertyType.Additional ? AdditionalColumnTypes.FirstOrDefault(item => item.UID == ((ElementPassCardTextProperty)ElementTextBlock).AdditionalColumnUID) : null;
		}
		protected override bool Save()
		{
			Text = SelectedPropertyType.ToDescription();
			if (SelectedPropertyType == PassCardTextPropertyType.Additional)
				Text += string.Format("\n({0})", SelectedAdditionalColumnType == null ? string.Empty : SelectedAdditionalColumnType.Name);
			var element = (ElementPassCardTextProperty)ElementTextBlock;
			element.PropertyType = SelectedPropertyType;
			element.AdditionalColumnUID = SelectedAdditionalColumnType == null ? Guid.Empty : SelectedAdditionalColumnType.UID;
			return base.Save();
		}
	}
}