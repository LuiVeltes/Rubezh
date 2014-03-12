﻿using System;
using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DelaysViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Delays = new List<DelayViewModel>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					var delayViewModel = new DelayViewModel(delay);
					Delays.Add(delayViewModel);
				}
			}
			SelectedDelay = Delays.FirstOrDefault();
		}

		List<DelayViewModel> _delay;
		public List<DelayViewModel> Delays
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged("Delays");
			}
		}

		DelayViewModel _selectedDelay;
		public DelayViewModel SelectedDelay
		{
			get { return _selectedDelay; }
			set
			{
				_selectedDelay = value;
				OnPropertyChanged("SelectedDelay");
			}
		}

		public void Select(Guid delayUID)
		{
			if (delayUID != Guid.Empty)
			{
				SelectedDelay = Delays.FirstOrDefault(x => x.Delay.BaseUID == delayUID);
			}
		}
	}
}