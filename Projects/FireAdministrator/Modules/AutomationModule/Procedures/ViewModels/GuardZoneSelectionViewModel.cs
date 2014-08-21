﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GuardZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public GuardZoneSelectionViewModel(XGuardZone guardZone)
		{
			Title = "Выбор охранной зоны";
			Zones = new ObservableCollection<ZoneViewModel>();
			XManager.GuardZones.ForEach(x => Zones.Add(new ZoneViewModel(x)));
			if (guardZone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.GuardZone.UID == guardZone.UID);
			if (SelectedZone == null)
				SelectedZone = Zones.FirstOrDefault();
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}