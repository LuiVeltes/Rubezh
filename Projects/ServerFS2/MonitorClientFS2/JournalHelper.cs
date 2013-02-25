﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();
		private static int _usbRequestNo;

		private static FSJournalItem ReadItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			var data = SendByteCommand(bytes, device);
			if (data != null)
			{
				lock (Locker)
				{
					return JournalParser.FSParce(data.Data);
				}
			}
			else
			{
				Trace.WriteLine("SendCode(bytes).Result.FirstOrDefault() == null");
				return null;
			}
		}

		public static List<FSJournalItem> GetSecJournalItems2Op(Device device)
		{
			int lastindex = GetLastSecJournalItemId2Op(device);
			var journalItems = new List<FSJournalItem>();
			for (int i = 0; i <= lastindex; i++)
			{
				var bytes = new List<byte> { 0x20, 0x02 };
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				var data = SendByteCommand(bytes, device);
				if (data != null)
					lock (Locker)
					{
						journalItems.Add(JournalParser.FSParce(data.Data));
					}
				else
					Trace.WriteLine("SendCode(bytes).Result.FirstOrDefault() == null");
			}
			journalItems = journalItems.OrderByDescending(x => x.SystemTime).ToList();
			return journalItems;
		}

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			try
			{
				var lastindex = SendByteCommand(new List<byte> { 0x21, 0x02 }, device);
				int li = 256 * lastindex.Data[9] + lastindex.Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return GetLastJournalItemId(device) + GetLastSecJournalItemId2Op(device);
			try
			{
				var firecount = SendByteCommand(new List<byte> { 0x24, 0x00 }, device);
				int fc = 256 * firecount.Data[7] + firecount.Data[8];
				return fc;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetFirstJournalItemId(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return 0;
			return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			try
			{
				var lastindex = SendByteCommand(new List<byte> { 0x21, 0x00 }, device);
				int li = 256 * lastindex.Data[9] + lastindex.Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static List<FSJournalItem> GetAllJournalItems(Device device)
		{
			return GetJournalItems(device, GetLastJournalItemId(device), GetFirstJournalItemId(device));
		}

		public static List<FSJournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FSJournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadItem(device, i));
			if (device.Driver.DriverType == DriverType.Rubezh_2OP)
			{
				journalItems.AddRange(GetSecJournalItems2Op(device));
			}
			return journalItems;
		}

		private static ServerFS2.Response SendByteCommand(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			//bytes.Add(Convert.ToByte(device.IntAddress / 256));
			bytes.Add(0x03); // 1 шлейф
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
		}
	}
}