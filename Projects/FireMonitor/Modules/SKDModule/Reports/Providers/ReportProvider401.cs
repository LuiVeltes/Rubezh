﻿using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;

namespace SKDModule.Reports.Providers
{
    public class ReportProvider401 : FilteredSKDReportProvider<SKDReportFilter>
	{
		public ReportProvider401()
            : base("Report401", "401. Отчет по событиям системы контроля доступа", 401, SKDReportGroup.Events)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "c01", "Дата и время в системе" },
					{ "c02", "Дата и время на устройстве" },
					{ "c03", "Название" },
					{ "c04", "Объект" },
					{ "c05", "Пользователь" },
					{ "c06", "Подсистема" },
				},
			};
		}
	}
}
