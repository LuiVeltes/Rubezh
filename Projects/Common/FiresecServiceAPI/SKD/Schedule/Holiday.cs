﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Holiday : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public HolidayType Type { get; set; }

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember]
		public DateTime? TransferDate { get; set; }

		[DataMember]
		public int? Reduction { get; set; }
	}

	public enum HolidayType
	{
		Holiday,
		Reduced,
		Transferred,
		Working
	}
}