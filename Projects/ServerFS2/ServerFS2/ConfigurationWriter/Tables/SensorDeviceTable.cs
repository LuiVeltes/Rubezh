﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class SensorDeviceTable : TableBase
	{
		public Device Device { get; private set; }

		public override Guid UID
		{
			get { return Device.UID; }
		}

		public SensorDeviceTable(FlashDatabase flashDatabase, Device device)
			: base(flashDatabase, device.PresentationAddressAndName)
		{
			Device = device;
		}

		public override void Create()
		{
			BytesDatabase.AddByte(Device.AddressOnShleif, "Адрес");
			BytesDatabase.AddByte(Math.Max(Device.ShleifNo - 1, 0), "Шлейф");
			BytesDatabase.AddShort(0, "Внутренние параметры", true);
			BytesDatabase.AddByte(0, "Динамические параметры для базы", true);
			var zoneNo = 0;
			if (Device.Zone != null)
			{
				var binaryZone = PanelDatabase.BinaryPanel.BinaryLocalZones.FirstOrDefault(x => x.Zone == Device.Zone);
				if (binaryZone != null)
				{
					zoneNo = binaryZone.LocalNo;
				}
			}
			BytesDatabase.AddShort(zoneNo, "Номер зоны");
			var lengtByteDescription = BytesDatabase.AddByte(0, "Длина блока данных устройства");
			//if (!IsAm())
			//{
			//    BytesDatabase.AddByte(0, "Пустой байт");
			//}
			//for (int i = 0; i < Get80ByteCount(); i++)
			for (int i = 0; i < 2; i++)
			{
				BytesDatabase.AddByte(0, "Байт 0x80 или 0x81");
			}
			AddDetectorProperties();

			switch (Device.Driver.DriverType)
			{
				case DriverType.AM_1:
				case DriverType.StopButton:
				case DriverType.StartButton:
				case DriverType.AutomaticButton:
				case DriverType.ShuzOnButton:
				case DriverType.ShuzOffButton:
				case DriverType.ShuzUnblockButton:
					AddAM1Config();
					break;

				case DriverType.AMT_4:
					AddAMT4Config();
					break;

				case DriverType.AM1_T:
					AddAM1_TConfig();
					break;

				case DriverType.AMP_4:
					AddAMP_4Config();
					break;

				case DriverType.AM1_O:
					AddAM1_OConfig();
					break;

				case DriverType.PowerSupply:
					AddPowerSupplyConfig();
					break;
			}
			lengtByteDescription.Value = BytesDatabase.ByteDescriptions.Count - 8;
		}

		//int Get80ByteCount()
		//{
		//    if(IsAm())
		//            return 1;
		//    return 2;
		//}

		void AddDetectorProperties()
		{
			byte computerConfigurationData = 0;

			switch (Device.Driver.DriverType)
			{
				case DriverType.SmokeDetector:
					BytesDatabase.AddByte(0, "Запыленность", true);
					break;
				case DriverType.HeatDetector:
					BytesDatabase.AddByte(0, "Температура", true);
					break;
				case DriverType.CombinedDetector:
					BytesDatabase.AddByte(computerConfigurationData, "Конфигурация с компа");
                    BytesDatabase.AddByte(0, "Запыленность", true);
                    BytesDatabase.AddByte(0, "Температура", true);

                    BytesDatabase.AddByte(0, "Пустой байт", true);
                    BytesDatabase.AddByte(0, "Пустой байт", true);
					break;
				case DriverType.RadioSmokeDetector:
                    BytesDatabase.AddByte(0, "Запыленность", true);
					BytesDatabase.AddByte(Device.Parent.AddressOnShleif, "Адрес родительского МРК");
					break;
				case DriverType.RadioHandDetector:
					BytesDatabase.AddByte(Device.Parent.AddressOnShleif, "Адрес родительского МРК");
					break;
			}
		}

		void AddAM1Config()
		{
			var config = 0;
			if (Device.Parent.Driver.IsGroupDevice)
			{
				var childIndex = Device.Parent.Children.IndexOf(Device);
				childIndex = childIndex << 1;
				config += 16 + childIndex;
			}
			BytesDatabase.AddByte(config, "Конфиг с компа");

			var amVitualType = GetAMVitualType();
			BytesDatabase.AddByte(amVitualType, "ID виртуального типа");

			BytesDatabase.AddShort(0, "Общее оличество привязаннфх к сработке ИУ");

			//BytesDatabase.AddShort(0, "Общее количество привязанных к сработке виртуальных кнопок ИУ");
			//for (int i = 0; i < ParentPanel.Driver.ShleifCount; i++)
			//{
			//    BytesDatabase.AddByte(0, "Количество связанных ИУ шлейфа " + (i+1).ToString());
			//    BytesDatabase.AddReference((ByteDescription)null, "Указатель на размещение абсолютного адреса размещения первого в списке связанного ИУ шлейфа " + (i + 1).ToString());
			//}
		}

		void AddAMT4Config()
		{
			var config = 0;
			BytesDatabase.AddByte(config, "Конфиг с компа");
			BytesDatabase.AddByte(0, "Байт конфига порого минимума");
			BytesDatabase.AddByte(0, "Байт конфига порого максимума");
			BytesDatabase.AddByte(0, "Максимум измеряемого датчиком давления");
		}

		void AddAM1_TConfig()
		{
			var config = 0;
			BytesDatabase.AddByte(config, "Конфиг с компа");

			var description = Device.Description;
			if (string.IsNullOrEmpty(description))
			{
				description = Device.Driver.ShortName + " 0." + Device.PresentationAddress;
				if (Device.Driver.DriverType == DriverType.Exit)
				{
					description = "Выход 0." + Device.Parent.IntAddress.ToString() + "." + Device.AddressOnShleif.ToString();
				}
			}
			BytesDatabase.AddString(description, "Описание");

			var event1PropertyValue = "";
			var event1Property = Device.Properties.FirstOrDefault(x => x.Name == "Event1");
			if (event1Property != null)
			{
				event1PropertyValue = event1Property.Value;
			}
			var event2PropertyValue = "";
			var event2Property = Device.Properties.FirstOrDefault(x => x.Name == "Event2");
			if (event2Property != null)
			{
				event2PropertyValue = event2Property.Value;
			}
			BytesDatabase.AddString(event1PropertyValue, "Описание сработавшего состояния");
			BytesDatabase.AddString(event2PropertyValue, "Описание состояния норма");

			var rmDevices = new HashSet<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
                    if (clause.Device != null && clause.Device == Device && clause.Device.Driver.DriverType == DriverType.AM1_T)
					{
						rmDevices.Add(device);
					}
				}
			}

			BytesDatabase.AddShort(rmDevices.Count, "Общее количество привязанных к сработке виртуальных кнопок ИУ");
			if (rmDevices.Count > 0)
			{
				foreach (var rmDevice in rmDevices)
				{
					TableBase table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == rmDevice.UID);
					BytesDatabase.AddReferenceToTable(table, "Указатель на размещение абсолютного адреса размещения связанного ИУ " + rmDevice.DottedPresentationAddressAndName);
				}
				//for (int i = 0; i < ParentPanel.Driver.ShleifCount; i++)
				//{
				//    BytesDatabase.AddByte(0, "Количество связанных ИУ шлейфа " + (i + 1).ToString());
				//    BytesDatabase.AddReference((ByteDescription)null, "Указатель на размещение абсолютного адреса размещения первого в списке связанного ИУ шлейфа " + (i + 1).ToString());
				//}
			}
		}

		void AddAMP_4Config()
		{
			var config = 0;
			if (Device.Parent.Driver.IsGroupDevice)
			{
				var childIndex = Device.Parent.Children.IndexOf(Device);
				childIndex = childIndex << 1;
				config += 16 + childIndex;
			}
			BytesDatabase.AddByte(config, "Конфиг с компа");
		}

		void AddAM1_OConfig()
		{
			BytesDatabase.AddByte(0, "Неиспользуемый байт, можно убрать с инкрементом версии базы");
			BytesDatabase.AddByte(0, "Байт подтипа извещателя");
			var config = 0;
			BytesDatabase.AddByte(config, "Конфиг с компа");
		}

		void AddPowerSupplyConfig()
		{
			BytesDatabase.AddString(Device.Description, "Описание");
		}

		int GetAMVitualType()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.AM_1:
					return 0x51;
				case DriverType.StopButton:
					return 0x52;
				case DriverType.AutomaticButton:
					return 0x53;
				case DriverType.StartButton:
					return 0x54;
				case DriverType.ShuzOnButton:
					return 0x58;
				case DriverType.ShuzOffButton:
					return 0x59;
				case DriverType.ShuzUnblockButton:
					return 0x5A;
			}
			return 0;
		}

		bool IsAm()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.AMT_4:
				case DriverType.AM1_T:
				case DriverType.AM_1:
				case DriverType.AM1_O:
				case DriverType.StopButton:
				case DriverType.StartButton:
				case DriverType.AutomaticButton:
				case DriverType.ShuzOnButton:
				case DriverType.ShuzOffButton:
				case DriverType.ShuzUnblockButton:
				case DriverType.AMP_4:
					return true;
			}
			return false;
		}
	}
}