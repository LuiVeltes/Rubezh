﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

namespace GKModule.ViewModels
{
	public class JournalColumnTypeViewModel : BaseViewModel
	{
		public JournalColumnTypeViewModel(JournalColumnType journalColumnType)
		{
			JournalColumnType = journalColumnType;
		}

		public JournalColumnType JournalColumnType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}

