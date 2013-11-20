﻿using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_RM_1_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xDA,
				DriverType = XDriverType.RSR2_RM_1,
				UID = new Guid("58C2A881-783F-4638-A27C-42257D5B31F9"),
				Name = "Релейный исполнительный модуль МР RSR2",
				ShortName = "МР RSR2",
				IsControlDevice = true,
				HasLogic = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddControlAvailableStates(driver);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.AutoOff);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.On);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.TurningOn);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Off);


			driver.AvailableCommandBits.Add(XStateBit.TurnOn_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOnNow_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOff_InManual);
			driver.AvailableCommandBits.Add(XStateBit.TurnOffNow_InManual);

			GKDriversHelper.AddIntProprety(driver, 0, "Задержка на включение, с", 10, 0, 65535);
			GKDriversHelper.AddIntProprety(driver, 1, "Время удержания, с", 1, 1, 65535);
			GKDriversHelper.AddIntProprety(driver, 2, "Задержка на выключение, с", 1, 1, 65535);

			var property1 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние контакта для режима Выключено",
				Caption = "Состояние контакта для режима Выключено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x03
			};
			GKDriversHelper.AddPropertyParameter(property1, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property1, "Контакт НЗ", 1);
			GKDriversHelper.AddPropertyParameter(property1, "Контакт переключается", 2);
			driver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние контакта для режима Удержания",
				Caption = "Состояние контакта для режима Удержания",
				Default = 0,
				IsLowByte = true,
				Mask = 0x0C
			};
			GKDriversHelper.AddPropertyParameter(property2, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property2, "Контакт НЗ", 4);
			GKDriversHelper.AddPropertyParameter(property2, "Контакт переключается", 8);
			driver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 3,
				Name = "Состояние контакта для режима Включено",
				Caption = "Состояние контакта для режима Включено",
				Default = 0,
				IsLowByte = true,
				Mask = 0x30
			};
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НР", 0);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт НЗ", 16);
			GKDriversHelper.AddPropertyParameter(property3, "Контакт переключается", 32);
			driver.Properties.Add(property3);

			driver.AUParameters.Add(new XAUParameter() { No = 1, Name = "Отсчет задержки на включение, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 2, Name = "Отсчет удержания, с", IsDelay = true });
			driver.AUParameters.Add(new XAUParameter() { No = 3, Name = "Отсчет задержки на выключение, с", IsDelay = true });

			return driver;
		}
	}
}