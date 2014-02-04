﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.ViewModels;
using FiresecAPI;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDZone Zone { get; private set; }

		public ZoneDetailsViewModel(SKDZone zone = null)
		{
			if (zone == null)
			{
				Title = "Создание новой зоны";
				Zone = new SKDZone()
				{
					Name = "Новая зона",
				};
			}
			else
			{
				Title = string.Format("Свойства зоны: {0}", zone.Name);
				Zone = zone;
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Zone.Name;
			Description = Zone.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		protected override bool Save()
		{
			Zone = new SKDZone()
			{
				Name = Name,
				Description = Description
			};
			return true;
		}
	}
}