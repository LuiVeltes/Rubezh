﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMPT : XBase, INamedBase
	{
		public XMPT()
		{
			StartLogic = new XDeviceLogic();
			MPTDevices = new List<MPTDevice>();
			Delay = 10;

			Devices = new List<XDevice>();
		}

		public List<XDevice> Devices { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Delay { get; set; }

		[DataMember]
		public int Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public List<MPTDevice> MPTDevices { get; set; }

		[DataMember]
		public bool UseDoorAutomatic { get; set; }

		[DataMember]
		public bool UseDoorStop { get; set; }

		[DataMember]
		public bool UseFailureAutomatic { get; set; }

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.MPT; } }

		public override string PresentationName
		{
			get { return "MПТ" + "." + Name; }
		}
	}
}