﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyIntervalPart
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public bool IsHolliday { get; set; }

		[DataMember]
		public int TimeIntervalID { get; set; }

		[DataMember]
		public Guid HolidayUID { get; set; }
	}
}