﻿using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_MVP_Part_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_MVP_Part,
				UID = new Guid("118A822D-25F0-429a-A1E8-3EFEB40900E0"),
				Name = "Линия МВП",
				ShortName = "Линия МВП",
				IsAutoCreate = true,
				MinAddress = 1,
				MaxAddress = 2,
				HasAddress = false,
				IsReal = false
			};

			GKDriversHelper.AddIntProprety(driver, 0, "Число АУ на АЛС", 0, 0, 250);
			return driver;
		}
	}
}