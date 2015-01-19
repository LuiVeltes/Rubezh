﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ShortEmployee : IOrganisationElement
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public bool IsDepartmentDeleted { get; set; }

		[DataMember]
		public string PositionName { get; set; }

		[DataMember]
		public bool IsPositionDeleted { get; set; }

		[DataMember]
		public List<SKDCard> Cards { get; set; }

		[DataMember]
		public PersonType Type { get; set; }

		[DataMember]
		public List<TextColumn> TextColumns { get; set; }

		[DataMember]
		public string CredentialsStartDate { get; set; }

		[DataMember]
		public int TabelNo { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public string OrganisationName { get; set; }

		[DataMember]
		public string Phone { get; set; }

		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }

		[DataMember]
		public DateTime LastEmployeeDayUpdate { get; set; }

		[DataMember]
		public Guid ScheduleUID { get; set; }

		public string Name 
		{ 
			get { return FIO; } 
			set { return; }
		}

		public string FIO
		{
			get
			{
				return LastName + " " + FirstName + (SecondName != null ? " " + SecondName : "");
			}
		}
	}

	public class TextColumn
	{
		public Guid ColumnTypeUID { get; set; }
		public string Text { get; set; }
	}
}