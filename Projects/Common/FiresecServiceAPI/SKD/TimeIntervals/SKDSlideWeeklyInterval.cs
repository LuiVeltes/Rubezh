﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDSlideWeeklyInterval
	{
		public SKDSlideWeeklyInterval()
		{
			UID = Guid.NewGuid();
			WeeklyIntervalUIDs = new List<Guid>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<Guid> WeeklyIntervalUIDs { get; set; }
	}
}