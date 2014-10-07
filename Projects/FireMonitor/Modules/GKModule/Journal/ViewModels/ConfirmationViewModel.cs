﻿using System;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		DateTime StartDateTime = DateTime.Now;

		public ConfirmationViewModel(GKJournalItem journalItem)
		{
			Title = "Подтверждение критических событий";
			ConfirmCommand = new RelayCommand(OnConfirm);
			JournalItemViewModel = new JournalItemViewModel(journalItem);
		}

		public JournalItemViewModel JournalItemViewModel { get; private set; }

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			var deltaSeconds = (int)(DateTime.Now - StartDateTime).TotalSeconds;
			FiresecManager.FiresecService.GKAddMessage(JournalEventNameType.Подтверждение_тревоги,
				JournalItemViewModel.JournalItem.Name + " " + JournalItemViewModel.JournalItem.Description +
				" (время реакции " + deltaSeconds.ToString() + " сек)");
			Close();
		}
	}
}