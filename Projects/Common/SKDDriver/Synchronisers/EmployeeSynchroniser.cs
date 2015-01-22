﻿using System;
using System.Data.Linq;
using System.Linq.Expressions;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class EmployeeSynchroniser : Synchroniser<ExportEmployee, DataAccess.Employee>
	{
		public EmployeeSynchroniser(Table<DataAccess.Employee> table, SKDDatabaseService databaseService) : base(table, databaseService) { }

		public override ExportEmployee Translate(DataAccess.Employee item)
		{
			return new ExportEmployee 
			{ 
				FirstName = item.FirstName, 
				SecondName = item.SecondName,
				LastName = item.LastName,
				CredentialsStartDate = item.CredentialsStartDate,
				DocumentNumber = item.DocumentNumber,
				BirthDate = item.BirthDate,
				BirthPlace = item.BirthPlace,
				DocumentGivenDate = item.DocumentGivenDate,
				DocumentGivenBy = item.DocumentGivenBy,
				DocumentValidTo = item.DocumentValidTo,
				DocumentDepartmentCode = item.DocumentDepartmentCode,
				Citizenship = item.Citizenship,
				Description = item.Description,
				Gender = item.Gender,
				Type = item.Type != null ? item.Type.Value : -1,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation),
				PositionUID = GetUID(item.PositionUID),
				PositionExternalKey = GetExternalKey(item.PositionUID, item.Position),
				DepartmentUID = GetUID(item.DepartmentUID),
				DepartmentExternalKey = GetExternalKey(item.DepartmentUID, item.Department),
				EscrortUID = GetUID(item.EscortUID),
				EscortExternalKey = GetExternalKey(item.EscortUID, item.Employee1)
			};
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(Guid uid)
		{
			return base.IsInFilter(uid).And(x => x.OrganisationUID == uid);
		}

		public override void TranslateBack(ExportEmployee exportItem, DataAccess.Employee tableItem)
		{
			tableItem.FirstName = exportItem.FirstName;
			tableItem.SecondName = exportItem.SecondName;
			tableItem.LastName = exportItem.LastName;
			tableItem.CredentialsStartDate = TranslatiorHelper.CheckDate(exportItem.CredentialsStartDate);
			tableItem.DocumentNumber = exportItem.DocumentNumber;
			tableItem.BirthDate = TranslatiorHelper.CheckDate(tableItem.BirthDate);
			tableItem.BirthPlace = tableItem.BirthPlace;
			tableItem.DocumentGivenDate = TranslatiorHelper.CheckDate(exportItem.DocumentGivenDate);
			tableItem.DocumentGivenBy = exportItem.DocumentGivenBy;
			tableItem.DocumentValidTo = TranslatiorHelper.CheckDate(exportItem.DocumentValidTo);
			tableItem.DocumentDepartmentCode = exportItem.DocumentDepartmentCode;
			tableItem.Citizenship = exportItem.Citizenship;
			tableItem.Description = exportItem.Description;
			tableItem.Gender = exportItem.Gender;
			tableItem.Type = exportItem.Type;

			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
			tableItem.PositionUID = GetUIDbyExternalKey(exportItem.PositionExternalKey, _DatabaseService.Context.Positions);
			tableItem.DepartmentUID = GetUIDbyExternalKey(exportItem.DepartmentExternalKey, _DatabaseService.Context.Departments);
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportEmployee"; }
		}
		
		protected override string Name
		{
			get { return "Employees"; }
		}
	}
}


