﻿using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailConfigViewModel : SaveCancelDialogViewModel
	{
		public EmailSettingsViewModel EmailSettingsParamsViewModel { get; private set; }

		public EmailConfigViewModel()
		{
			EmailSettingsParamsViewModel = new EmailSettingsViewModel();
			SetDefaultEmailSettingsCommand = new RelayCommand(OnSetDefaultEmailSettings);
		}

		public EmailConfigViewModel(EmailSettings senderParams)
		{
			EmailSettingsParamsViewModel = new EmailSettingsViewModel(senderParams);
			SetDefaultEmailSettingsCommand = new RelayCommand(OnSetDefaultEmailSettings);
		}

		public RelayCommand SetDefaultEmailSettingsCommand { get; set; }

		private void OnSetDefaultEmailSettings()
		{
			EmailSettingsParamsViewModel.EmailSettings = EmailSettings.SetDefaultParams();
			EmailSettingsParamsViewModel.Update();
		}
	}
}