﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
{
	public class PumpStationCreator
	{
		GkDatabase GkDatabase;
		List<XDirection> Directions;
		List<XDevice> FirePumpDevices;
		List<XDevice> FailurePumpDevices;
		List<XDelay> Delays;
		ushort DelayTime;
		ushort PumpsCount;
		XDelay MainDelay;

		public PumpStationCreator(GkDatabase gkDatabase, XDevice pumpStatioDevice)
		{
			if (pumpStatioDevice.PumpStationProperty == null)
				pumpStatioDevice.PumpStationProperty = new XPumpStationProperty();

			GkDatabase = gkDatabase;
			DelayTime = pumpStatioDevice.PumpStationProperty.DelayTime;
			PumpsCount = pumpStatioDevice.PumpStationProperty.PumpsCount;

			Directions = new List<XDirection>();
			foreach (var directionUID in pumpStatioDevice.PumpStationProperty.DirectionUIDs)
			{
				var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
				if (direction != null)
				{
					Directions.Add(direction);
				}
			}

			Delays = new List<XDelay>();

			FirePumpDevices = new List<XDevice>();
			foreach (var pumpDeviceUID in pumpStatioDevice.PumpStationProperty.FirePumpUIDs)
			{
				var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpDeviceUID);
				if (pumpDevice != null)
				{
					var addressOnShleif = pumpDevice.IntAddress % 256;
					if (addressOnShleif <= 8)
					{
						FirePumpDevices.Add(pumpDevice);
					}
				}
			}

			FailurePumpDevices = new List<XDevice>();
			var drenajPump = XManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == pumpStatioDevice.PumpStationProperty.DrenajPumpUID);
			if (drenajPump != null)
			{
				FailurePumpDevices.Add(drenajPump);
			}
			var jokeyPump = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpStatioDevice.PumpStationProperty.JokeyPumpUID);
			if (jokeyPump != null)
			{
				FailurePumpDevices.Add(jokeyPump);
			}
			var compressorPump = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpStatioDevice.PumpStationProperty.CompressorPumpUID);
			if (compressorPump != null)
			{
				FailurePumpDevices.Add(compressorPump);
			}
			var compensationPump = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpStatioDevice.PumpStationProperty.CompensationPumpUID);
			if (compensationPump != null)
			{
				FailurePumpDevices.Add(compensationPump);
			}
		}

		public void Create()
		{
			CreateMainDelay();
			CreateDelays();
			SetCrossReferences();

			foreach (var delay in Delays)
			{
				GkDatabase.AddDelay(delay);
				var deviceBinaryObject = new DelayBinaryObject(delay);
				GkDatabase.BinaryObjects.Add(deviceBinaryObject);
			}

			CreateMainDelayLogic();
			CreateDelaysLogic();
			SetPumpDevicesLogic();
		}

		void CreateMainDelay()
		{
			MainDelay = new XDelay()
			{
				Name = "Задержка пуска НС",
				DelayTime = (ushort)(0),
				SetTime = 2,
				DelayRegime = DelayRegime.On
			};
			Delays.Add(MainDelay);
		}

		void CreateDelays()
		{
			int pumpIndex = 1;
			foreach (var pumpDevice in FirePumpDevices)
			{
				var delay = new XDelay()
				{
					Name = "Задержка пуска ШУН " + pumpDevice.DottedAddress,
					DelayTime = (ushort)(pumpIndex * DelayTime),
					SetTime = 2,
					DelayRegime = DelayRegime.Off
				};

				Delays.Add(delay);
				pumpIndex++;
			}
		}

		void CreateMainDelayLogic()
		{
			var mainDelayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == MainDelay.UID);
			if (mainDelayBinaryObject != null)
			{
				var formula = new FormulaBuilder();

				if (Directions.Count > 0)
				{
					var inputDirectionsCount = 0;
					foreach (var direction in Directions)
					{
						formula.AddGetBit(XStateType.On, direction);
						if (inputDirectionsCount > 0)
						{
							formula.Add(FormulaOperationType.OR);
						}
						inputDirectionsCount++;
					}
					foreach (var failurePumpDevice in FailurePumpDevices)
					{
						formula.AddGetBit(XStateType.Failure, failurePumpDevice);
						formula.Add(FormulaOperationType.COM);
						formula.Add(FormulaOperationType.AND);
					}

					formula.AddPutBit(XStateType.TurnOn, MainDelay);
				}

				formula.Add(FormulaOperationType.END);
				mainDelayBinaryObject.Formula = formula;
				mainDelayBinaryObject.FormulaBytes = formula.GetBytes();
			}
		}

		void CreateDelaysLogic()
		{
			foreach (var delay in Delays)
			{
				if (delay.UID == MainDelay.UID)
					continue;

				var delayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == delay.UID);
				if (delayBinaryObject != null)
				{
					var formula = new FormulaBuilder();

					formula.AddGetBit(XStateType.On, MainDelay);
					formula.AddPutBit(XStateType.TurnOn, delay);
					formula.AddGetBit(XStateType.Off, MainDelay);
					formula.AddPutBit(XStateType.TurnOff, delay);

					formula.Add(FormulaOperationType.END);
					delayBinaryObject.Formula = formula;
					delayBinaryObject.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void SetPumpDevicesLogic()
		{
			foreach (var pumpDevice in FirePumpDevices)
			{
				var pumpBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpBinaryObject != null)
				{
					var formula = new FormulaBuilder();
					var inputPumpsCount = 0;
					foreach (var otherPumpDevice in FirePumpDevices)
					{
						if (otherPumpDevice.UID == pumpDevice.UID)
							continue;

						formula.AddGetBit(XStateType.TurningOn, otherPumpDevice);
						formula.AddGetBit(XStateType.On, otherPumpDevice);
						formula.Add(FormulaOperationType.OR);
						if (inputPumpsCount > 0)
						{
							formula.Add(FormulaOperationType.ADD); // почситать количество включеных насосов
						}
						inputPumpsCount++;
					}
					formula.Add(FormulaOperationType.CONST, 0, PumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateType.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса

					formula.AddGetBit(XStateType.On, MainDelay);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateType.TurnOn, pumpDevice); // включить насос

					formula.AddGetBit(XStateType.On, MainDelay);
					formula.Add(FormulaOperationType.COM);
					formula.AddGetBit(XStateType.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса
					formula.AddPutBit(XStateType.TurnOff, pumpDevice); // выключить насос
					formula.Add(FormulaOperationType.END);

					pumpBinaryObject.Formula = formula;
					pumpBinaryObject.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void SetCrossReferences()
		{
			foreach (var direction in Directions)
			{
				MainDelay.InputObjects.Add(direction);
				direction.OutputObjects.Add(MainDelay);
			}

			foreach (var pumpDevice in FirePumpDevices)
			{
				foreach (var delay in Delays)
				{
					pumpDevice.InputObjects.Add(delay);
					delay.OutputObjects.Add(pumpDevice);
				}
			}

			foreach (var delay in Delays)
			{
				if (delay.UID != MainDelay.UID)
				{
					delay.InputObjects.Add(MainDelay);
					MainDelay.OutputObjects.Add(delay);
				}
			}
		}
	}
}