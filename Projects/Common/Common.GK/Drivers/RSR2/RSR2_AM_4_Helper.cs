﻿using System;
using XFiresecAPI;

namespace Common.GK
{
	public static class RSR2_AM_4_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_AM_4,
				UID = new Guid("79EAC50A-D534-4775-A102-BE4872877400"),
				Name = "Пожарная адресная метка АМ-4 RSR2",
				ShortName = "АМ-4 RSR2",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RSR2_AM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}