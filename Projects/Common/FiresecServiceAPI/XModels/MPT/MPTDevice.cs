﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class MPTDevice
	{
		public MPTDevice()
		{
			MPTDeviceType = MPTDeviceType.Unknown;
		}

		[XmlIgnore]
		public XDevice Device { get; set; }

		public Guid DeviceUID { get; set; }
		public MPTDeviceType MPTDeviceType { get; set; }

		public static List<XDriverType> GetAvailableMPTDriverTypes(MPTDeviceType mptDeviceType)
		{
			var result = new List<XDriverType>();
			switch (mptDeviceType)
			{
				case MPTDeviceType.DoNotEnterBoard:
				case MPTDeviceType.ExitBoard:
				case MPTDeviceType.AutomaticOffBoard:
					result.Add(XDriverType.RSR2_OPS);
					result.Add(XDriverType.RSR2_OPK);
					result.Add(XDriverType.RSR2_RM_1);
					result.Add(XDriverType.RSR2_MVK8);
					break;

				case MPTDeviceType.Speaker:
					result.Add(XDriverType.RSR2_OPZ);
					result.Add(XDriverType.RSR2_OPK);
					break;

				case MPTDeviceType.Door:
				case MPTDeviceType.HandStart:
				case MPTDeviceType.HandStop:
				case MPTDeviceType.HandAutomatic:
					result.Add(XDriverType.RSR2_AM_1);
					break;

				case MPTDeviceType.Bomb:
					result.Add(XDriverType.RSR2_RM_1);
					result.Add(XDriverType.RSR2_MVK8);
					break;
			}
			return result;
		}
	}
}