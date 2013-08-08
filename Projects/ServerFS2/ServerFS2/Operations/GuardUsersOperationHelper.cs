﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.Operations
{
	public static class GuardUsersOperationHelper
	{
		public static List<GuardUser> DeviceGetGuardUsers(Device device)
		{
			var guardUsers = new List<GuardUser>();
			var result = new List<byte>();

			for (var i = 0x14000; i < 0x14E00; i += 0x100)
			{
				var response = USBManager.Send(device, "Чтение охранных пользователей", 0x01, 0x52, BitConverter.GetBytes(i).Reverse(), Math.Min(0xFF, 0x14E00 - i));
			    if (response.HasError)
			        return null;
			    result.AddRange(response.Bytes);
			}

			var guardUsersCount = result[0];
			var guardUsersBytes = result.GetRange(1, 13);
			for (int i = 0; i < guardUsersCount; i++)
			{
				var guardUser = new GuardUser();
				guardUser.CanUnSetZone = Convert.ToBoolean(result[i * 46 + 14]);
				guardUser.CanSetZone = Convert.ToBoolean(result[i * 46 + 14] >> 1);
				guardUser.Name = BytesHelper.BytesToString(result.GetRange(i*46 + 15, 20));
				guardUser.KeyTM = BytesHelper.BytesToString(result.GetRange(i*46 + 35, 6));
				guardUser.Password = BytesHelper.BytesToString(result.GetRange(i * 46 + 41, 3));
				var guardZonesBytes = result.GetRange(i*46 + 44, 16);
				var localZones = GetLocalZones(device);
				for (int j = 0; j < guardZonesBytes.Count; j ++)
				{

				}
			}
			return guardUsers;
		}

		public static void DeviceSetGuardUsers(Device device, List<GuardUser> guardUsers)
		{
			SetConfigurationOperationHelper.ConfirmLongTermOperation(device);
			// Данные таблицы
			var guardUsersCount = (byte) guardUsers.Count; // 1 байт
			var guardUsersBytes = GetGuardUsersByte(guardUsers);

			var bytes = USBManager.CreateBytesArray(guardUsersCount, guardUsersBytes);
			foreach (var guardUser in guardUsers)
			{
				bytes.AddRange(GuardUserDataToBytesList(device, guardUser));
			}

			// Добавление пустых пользователей
			var emptyGuardUser = new GuardUser();
			emptyGuardUser.KeyTM = new string('0', 12);
			emptyGuardUser.Password = "";
			emptyGuardUser.ZoneUIDs = new List<Guid>();

			for(int i = guardUsers.Count; i < 80; i++)
			{
				bytes.AddRange(GuardUserDataToBytesList(device, emptyGuardUser));
			}

			var begin = 0x14000;
			for (int i = 0; i < bytes.Count; i = i + 0x100)
			{
				USBManager.Send(device, "Запись охранных пользователей", 0x02, 0x52, BitConverter.GetBytes(begin + i).Reverse(), Math.Min(bytes.Count - i - 1, 0xFF), bytes.GetRange(i, Math.Min(bytes.Count - i, 0x100)));
			}
		}

		static List<byte> CreatePasswordBytes(string password)
		{
			var passwordByte = new List<byte>();
			var newPasswordString = new String('F',6);
			for (int i = 0; i < password.Length; i++)
			{
				if (i % 2 == 0)
				{
					newPasswordString = newPasswordString.Remove(i + 1, 1);
					newPasswordString = newPasswordString.Insert(i + 1, password[i].ToString());
				}
				else
				{
					newPasswordString = newPasswordString.Remove(i - 1, 1);
					newPasswordString = newPasswordString.Insert(i - 1, password[i].ToString());
				}
			}
			var newPasswordByte = BytesHelper.HexStringToByteArray(newPasswordString);
			return newPasswordByte;
		}

		static List<Zone> GetLocalZones(Device device)
		{
			var localZones = ConfigurationManager.Zones.Where(zone => zone.DevicesInZone.FirstOrDefault(x => x.Parent == device) != null).ToList();
			localZones.OrderBy(x => x.No);
			return localZones;
		}

		static List<byte> GetGuardUsersByte(List<GuardUser> guardUsers)
		{
			var guardUsersBytes = new List<byte>(13); // 13 байт

			for (int i = 0; i < 13; i++)
			{
				guardUsersBytes.Add(new byte());
				for (int j = 0; j < 8; j++)
				{
					var guardUser = guardUsers.FirstOrDefault(x => x.Id == (i*8 + j + 1));
					if (guardUser != null)
						guardUsersBytes[i] += (byte) (1 << (guardUser.Id - i*8 - 1));
				}
			}
			return guardUsersBytes;
		}

		static List<byte> GetGuardZones(Device device, GuardUser guardUser)
		{
			var guardZonesBytes = new List<byte>(16);
			var localZones = GetLocalZones(device);
			var guardZones = new List<Zone>();

			foreach (var zoneUID in guardUser.ZoneUIDs)
			{
				if (!guardZones.Any(x => x.UID == zoneUID))
					guardZones.Add(ConfigurationManager.Zones.FirstOrDefault(x => x.UID == zoneUID));
			}

		
			var localNos = new List<int>();
			foreach (var guardZone in guardZones)
			{
				var localNo = localZones.IndexOf(localZones.FirstOrDefault(x => x.UID == guardZone.UID));
				localNos.Add(localNo);
			}

			for (int i = 0; i < 16; i++)
			{
				guardZonesBytes.Add(new byte());
				for (int j = 0; j < 8; j++)
				{
					if (localNos.Any(x => x == (i * 8 + j)))
						guardZonesBytes[i] += (byte)(1 << j);
				}
			}

			return guardZonesBytes;
		}

		static byte GetUserAttribute(GuardUser guardUser)
		{
			return (byte)(Convert.ToByte(guardUser.CanSetZone) * 2 + Convert.ToByte(guardUser.CanUnSetZone));
		}

		static List<byte> GuardUserDataToBytesList(Device device, GuardUser guardUser)
		{
			var bytes = new List<byte>();
			// Даннные пользователей
			var guardUserAttribute = GetUserAttribute(guardUser);
			var guardUserName = BytesHelper.StringToBytes(guardUser.Name);
			var guardUserKeyTM = BytesHelper.HexStringToByteArray(guardUser.KeyTM);
			var guardUserPassword = CreatePasswordBytes(guardUser.Password);
			var guardZonesBytes = GetGuardZones(device, guardUser);

			bytes.AddRange(USBManager.CreateBytesArray(guardUserAttribute, guardUserName,
			guardUserKeyTM, guardUserPassword, guardZonesBytes));

			return bytes;
		}
	}
}