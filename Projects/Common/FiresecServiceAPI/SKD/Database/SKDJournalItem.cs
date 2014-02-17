﻿using System;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI
{
	[DataContract]
	public class SKDJournalItem//:SKDModelBase
	{
		public SKDJournalItem():base()
		{
			DeviceDateTime = DateTime.Now;
			SystemDateTime = DateTime.Now;
			DeviceStateClass = XStateClass.Norm;
			JournalItemType = JournalItemType.System;
		}
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public JournalItemType JournalItemType { get; set; }
		[DataMember]
		public DateTime? DeviceDateTime { get; set; }
		[DataMember]
		public DateTime? SystemDateTime { get; set; }
		[DataMember]
		public int? DeviceJournalRecordNo { get; set; }

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }
		[DataMember]
		public string DeviceName { get; set; }
		[DataMember]
		public int DeviceState { get; set; }
		[DataMember]
		public string IpAddress { get; set; }
		[DataMember]
		public XStateClass DeviceStateClass { get; set; }
		[DataMember]
		public int? CardNo { get; set; }
		[DataMember]
		public int? CardSeries { get; set; }
		[DataMember]
		public Guid? CardUid { get; set; }


		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public XSubsystemType SubsystemType { get; set; }
	}
}