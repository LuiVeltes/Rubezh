﻿using System.Data;
using CodeReason.Reports;
using FiresecAPI;
using GKModule.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Controls.Converters;

namespace GKModule.Reports
{
	internal class JournalReport : ISingleReportProvider, IFilterableReport
	{
		private ReportArchiveFilter ReportArchiveFilter { get; set; }
		public JournalReport()
		{
			ReportArchiveFilter = new ReportArchiveFilter();
		}

		#region IFilterableReport Members
		public void Filter(RelayCommand refreshCommand)
		{
			var archiveFilterViewModel = new ArchiveFilterViewModel(ReportArchiveFilter.ArchiveFilter);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
				refreshCommand.Execute();
			}
		}
		#endregion

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			ReportArchiveFilter.LoadArchive();
			var data = new ReportData();
			data.ReportDocumentValues.Add("StartDate", ReportArchiveFilter.StartDate);
			data.ReportDocumentValues.Add("EndDate", ReportArchiveFilter.EndDate);

			DataTable table = new DataTable("Journal");
			table.Columns.Add("DateTime");
			table.Columns.Add("Name");
			table.Columns.Add("YesNo");
			table.Columns.Add("Description");
			table.Columns.Add("ObjectName");
			table.Columns.Add("StateClass");
			foreach (var journalItem in ReportArchiveFilter.JournalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				var objectName = "";
				if (journalItemViewModel.DeviceState != null)
				{
					objectName = journalItemViewModel.DeviceState.Device.PresentationAddressAndDriver;
				}
				if (journalItemViewModel.ZoneState != null)
				{
					objectName = journalItemViewModel.ZoneState.Zone.PresentationName;
				}
				if (journalItemViewModel.DirectionState != null)
				{
					objectName = journalItemViewModel.DirectionState.Direction.PresentationName;
				}
				table.Rows.Add(
					journalItem.DeviceDateTime,
					journalItem.Name,
					journalItem.YesNo.ToDescription(),
					journalItem.Description,
					objectName,
					journalItem.StateClass.ToDescription());
			}
			data.DataTables.Add(table);
			return data;
		}
		#endregion

		#region IReportProvider Members
		public string Template
		{
			get { return "Reports/JournalReport.xaml"; }
		}

		public string Title
		{
			get { return "Журнал событий"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}
		#endregion
	}
}