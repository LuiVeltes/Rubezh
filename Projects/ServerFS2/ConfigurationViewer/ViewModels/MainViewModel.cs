﻿using ConfigurationViewer.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;
using System;
using System.Diagnostics;

namespace ConfigurationViewer.DataTemplates
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			WriteConfigurationCommand = new RelayCommand(OnWriteConfiguration, CanWriteConfiguration);
		}

		public RelayCommand WriteConfigurationCommand { get; private set; }
		private void OnWriteConfiguration()
		{
			var dateTime = DateTime.Now;
			var configurationWriterHelper = new SystemDatabaseCreator();
			configurationWriterHelper.Create();
			Trace.WriteLine("SystemDatabaseCreator TotalMilliseconds = " + (DateTime.Now - dateTime).TotalMilliseconds.ToString());
			return;

			var configurationDatabaseViewModel = new ConfigurationDatabaseViewModel(configurationWriterHelper);
			DialogService.ShowModalWindow(configurationDatabaseViewModel);
		}
		bool CanWriteConfiguration()
		{
			return true;
		}
	}
}